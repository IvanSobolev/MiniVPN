// Получаем параметры из URL
const urlParams = new URLSearchParams(window.location.search);
const questionsParam = urlParams.get('question');
const callbackUrlTemplate = urlParams.get('callbackUrl');
const requestType = (urlParams.get('method') || 'POST').toUpperCase(); // Тип запроса (по умолчанию POST)

// Парсим вопросы
const questions = questionsParam.split(',').map(q => {
    const [name, type] = q.split('.');
    return { name, type };
});

// Контейнер для вопросов
const questionsContainer = document.getElementById('questionsContainer');

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

    fieldRow.appendChild(label);
    fieldRow.appendChild(input);
    questionsContainer.appendChild(fieldRow);
});


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
    const formData = {};
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

    // Формируем URL для GET/DELETE
    let finalCallbackUrl = callbackUrlTemplate;
    if (requestType === 'GET' || requestType === 'DELETE') {
        // Замена параметров в URL
        finalCallbackUrl = callbackUrlTemplate.replace(/\{(\w+)\}/g, (_, key) => formData[key] || `{${key}}`);
        // Добавление остальных параметров в query string
        const queryParams = new URLSearchParams(formData).toString();
        if (queryParams) {
            finalCallbackUrl += `?${queryParams}`;
        }
    }

    // Отправляем запрос на сервер
    try {
        const fetchOptions = {
            method: requestType,
            headers: {
                'Content-Type': 'application/json',
            },
        };

        // Для POST и PUT добавляем тело запроса
        if (requestType === 'POST' || requestType === 'PUT') {
            fetchOptions.body = JSON.stringify(formData);
        }

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
