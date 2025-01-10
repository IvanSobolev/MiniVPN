// Получаем параметры из URL
const urlParams = new URLSearchParams(window.location.search);
const questionsParam = urlParams.get('question');
const callbackUrl = urlParams.get('callbackUrl');

// Парсим вопросы
const questions = questionsParam.split(',').map(q => {
    const [name, type] = q.split('.');
    return { name, type };
});

// Контейнер для вопросов
const questionsContainer = document.getElementById('questionsContainer');

// Динамически создаем поля для каждого вопроса
questions.forEach(({ name, type }) => {
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

    questionsContainer.appendChild(label);
    questionsContainer.appendChild(input);
});

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

    // Парсим initData и hash из callbackUrl
    const callbackParams = new URL(callbackUrl).searchParams;
    const initData = callbackParams.get('initData');
    const hash = callbackParams.get('hash');

    // Добавляем initData и hash в тело запроса
    formData.initData = initData;
    formData.hash = hash;

    // Отправляем данные на указанный в callbackUrl сервер
    try {
        const response = await fetch(callbackUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(formData),
        });

        if (response.ok) {
            alert('Form submitted successfully!');
        } else {
            alert('Error submitting the form.');
        }
    } catch (error) {
        console.error('Submission error:', error);
        alert('An error occurred while submitting the form.');
    }
});
