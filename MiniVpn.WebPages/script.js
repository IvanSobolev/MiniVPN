// Получаем параметры из URL
const urlParams = new URLSearchParams(window.location.search);
const questionsParam = urlParams.get('question');
const callbackUrlTemplate = urlParams.get('callbackUrl');
const getCurrentValueUrlTemplate = urlParams.get('getCurrentValueUrl');
const requestType = (urlParams.get('method') || 'POST').toUpperCase(); // Тип запроса (по умолчанию POST)

// Парсим вопросы
const questions = questionsParam.split(',').map(q => {
    const [name, type] = q.split('.');
    return { name, type };
});

// Контейнер для вопросов
const questionsContainer = document.getElementById('questionsContainer');

// Объект для хранения введенных данных
const formData = {};

// Динамически создаем поля для каждого вопроса
questions.forEach(({ name, type }) => {
    const fieldRow = document.createElement('div'); // Контейнер для строки
    fieldRow.className = 'field-row';

    const label = document.createElement('label');
    label.textContent = name;

    const input = document.createElement('input');
    input.name = name;

    switch (type) {
        case 'int':
            input.type = 'number';
            break;
        case 'datetime':
            input.type = 'datetime-local';
            break;
        case 'text':
            input.type = 'text';
            break;
        case 'bool':
            input.type = 'checkbox';
            break;
        default:
            input.type = 'text'; // По умолчанию текстовое поле
    }

    // При изменении сохраняем данные в объект formData
    input.addEventListener('input', () => {
        if (input.type === 'checkbox') {
            formData[name] = input.checked;
        } else {
            formData[name] = input.value;
        }
    });

    fieldRow.appendChild(label);
    fieldRow.appendChild(input);
    questionsContainer.appendChild(fieldRow);
});

// Добавляем кнопку "Загрузить текущие данные" только для PUT запросов
if (requestType === 'PUT') {
    const fetchButton = document.createElement('button');
    fetchButton.textContent = 'Загрузить текущие данные';
    fetchButton.type = 'button';
    fetchButton.style.marginBottom = '10px';
    document.getElementById('dynamicForm').appendChild(fetchButton);

    // Функция для выполнения запроса на получение текущих значений
    async function fetchCurrentValues() {
        let finalGetCurrentValueUrl = getCurrentValueUrlTemplate;

        // Заменяем параметры в URL
        finalGetCurrentValueUrl = finalGetCurrentValueUrl.replace(/\{(\w+)\}/g, (_, key) => formData[key] || `{${key}}`);

        try {
            const response = await fetch(finalGetCurrentValueUrl);
            if (response.ok) {
                const jsonResponse = await response.json();
                // Заполняем поля формы значениями из ответа
                questions.forEach(({ name }) => {
                    const inputField = document.querySelector(`input[name="${name}"]`);
                    if (inputField) {
                        inputField.value = jsonResponse[name] || '';
                        formData[name] = jsonResponse[name] || ''; // Сохраняем значения в formData
                    }
                });
            } else {
                console.error('Failed to fetch current values:', response.status);
            }
        } catch (error) {
            console.error('Error fetching current values:', error);
        }
    }

    // Добавляем обработчик на кнопку загрузки данных
    fetchButton.addEventListener('click', fetchCurrentValues);
}

// Функция для отображения JSON-ответа в удобном формате
function displayJsonResponse(responseJson, container) {
    container.innerHTML = ''; // Очищаем контейнер

    for (const [key, value] of Object.entries(responseJson)) {
        const keyElement = document.createElement('strong');
        keyElement.textContent = `${key}: `;

        const valueElement = document.createElement('div');
        valueElement.textContent = value;

        const wrapper = document.createElement('div');
        wrapper.style.marginBottom = '10px';
        wrapper.appendChild(keyElement);
        wrapper.appendChild(valueElement);

        container.appendChild(wrapper);
    }
}

// Обработка отправки формы
document.getElementById('dynamicForm').addEventListener('submit', async function(event) {
    event.preventDefault();

    // Сбор данных из формы
    const formElements = event.target.elements;
    Array.from(formElements).forEach(element => {
        if (element.name) {
            if (element.type === 'checkbox') {
                formData[element.name] = element.checked;
            } else {
                formData[element.name] = element.value;
            }
        }
    });

    // Ссылка на контейнер для отображения ответа
    const serverResponseDiv = document.getElementById('serverResponse');

    // Формируем URL для запроса
    let finalCallbackUrl = callbackUrlTemplate;
    finalCallbackUrl = callbackUrlTemplate.replace(/\{(\w+)\}/g, (_, key) => formData[key] || `{${key}}`);

    try {
        const fetchOptions = {
            method: requestType,
            headers: {
                'Content-Type': 'application/json',
            },
            body: requestType === 'POST' || requestType === 'PUT' ? JSON.stringify(formData) : undefined,
        };

        const response = await fetch(finalCallbackUrl, fetchOptions);

        // Проверяем тип ответа
        if (response.headers.get('Content-Type')?.includes('application/json')) {
            const jsonResponse = await response.json();

            // Отображаем JSON в удобном виде
            serverResponseDiv.style.display = 'block';
            serverResponseDiv.style.borderColor = '#4CAF50';
            serverResponseDiv.style.color = '#4CAF50';
            displayJsonResponse(jsonResponse, serverResponseDiv);
        } else {
            const responseText = await response.text();

            // Отображаем текстовый ответ
            serverResponseDiv.style.display = 'block';
            serverResponseDiv.textContent = `Success: ${responseText}`;
            serverResponseDiv.style.borderColor = '#4CAF50';
            serverResponseDiv.style.color = '#4CAF50';
        }
    } catch (error) {
        console.error('Submission error:', error);
        serverResponseDiv.style.display = 'block';
        serverResponseDiv.textContent = 'An error occurred while submitting the form.';
        serverResponseDiv.style.borderColor = '#f44336';
        serverResponseDiv.style.color = '#f44336';
    }
});
