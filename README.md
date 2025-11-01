Проект по „Системни шаблони“
Задание 8 — Експортиране на фактури за служебни разходи (C# / WinForms)
1. Описание на задачата

Да се създаде програма за експортиране на фактури за служебни разходи в различни формати: XML, JSON и CSV.
В зависимост от позицията на служителя, всеки разход се валидира по различен начин:
фиксирана максимална сума
процент от заплатата
custom формула, зададена от потребителя (пример: AMOUNT <= 0.4 * SALARY)

Програмата трябва да демонстрира използването на шаблоните:
Template pattern
Strategy
(Допълнителен) Factory

2. Използвани шаблони (Design Patterns)
Template pattern

Клас: ExpenseExporterTemplate

Този клас дефинира алгоритъма за експорт, който се състои от следните стъпки:

Валидиране на всеки разход спрямо съответния служител

Събиране на валидните разходи

Сериализация към избран формат (XML, JSON, CSV)

Запис във файл

Конкретните реализации (XmlExpenseExporter, JsonExpenseExporter, CsvExpenseExporter) променят само стъпката за сериализация, но използват една и съща обща структура.

Това е класически пример за Template pattern.

Strategy

Интерфейс: IValidationStrategy

Описва общия интерфейс за валидация на разходи.
Реализирани са три различни стратегии:

Клас							Описание
FixedAmountValidation			Проверява дали сумата не надвишава фиксиран лимит
PercentOfSalaryValidation		Проверява дали сумата не надвишава определен процент от заплатата
CustomFormulaValidation			Проверява дали сумата удовлетворява custom формула (например AMOUNT <= 0.4 * SALARY)

Така можем лесно да сменяме логиката за валидация без да променяме останалата част от кода.
Това демонстрира Strategy pattern.

Factory (допълнителен шаблон)

Клас: ValidationStrategyFactory

Фабриката отговаря за създаването на подходящата стратегия според типа валидация (ValidationMode) на служителя.

public static IValidationStrategy CreateFor(Employee employee)
{
    return employee.ValidationMode switch
    {
        ValidationMode.FixedAmount => new FixedAmountValidation(employee.MaxAllowedAmount),
        ValidationMode.PercentOfSalary => new PercentOfSalaryValidation(employee.MaxPercentOfSalary),
        ValidationMode.CustomFormula => new CustomFormulaValidation(employee.CustomFormula!),
        _ => new FixedAmountValidation(0)
    };
}

Това е Simple Factory, използван за създаване на правилния обект Strategy.

3. Графичен потребителски интерфейс (GUI)

Проектът е реализиран като WinForms приложение с лесен и интуитивен интерфейс.
Основна форма (MainForm)
Таблица със служители
Таблица с разходи
Бутон „Добави разход“
Избор на формат за експорт (XML / JSON / CSV)
Бутон „Експорт“

При натискане на „Експорт“ се:
Валидират всички разходи;
Записват валидните в избрания формат;
Показват грешки (ако има невалидни разходи) в диалогов прозорец.
Форма за добавяне на разход (AddExpenseForm)
Позволява да се избере служител, въведе описание, сума и дата на разход.

4. Структура на проекта
ExpenseExporterApp/
│
├── Models/
│   ├── Employee.cs
│   ├── Expense.cs
│   └── ValidationMode.cs
│
├── Validation/ -> Strategy + Factory
│   ├── IValidationStrategy.cs
│   ├── FixedAmountValidation.cs
│   ├── PercentOfSalaryValidation.cs
│   ├── CustomFormulaValidation.cs
│   └── ValidationStrategyFactory.cs
│
├── Export/ -> Template pattern
│   ├── ExpenseExporterTemplate.cs
│   ├── XmlExpenseExporter.cs
│   ├── JsonExpenseExporter.cs
│   └── CsvExpenseExporter.cs
│
└── UI/
    ├── MainForm.cs
    ├── AddExpenseForm.cs
    └── Designer файлове

5. UML диаграма
@startuml
namespace Models {
    class Employee {
        int Id
        string FullName
        string Position
        decimal Salary
        ValidationMode ValidationMode
        decimal MaxAllowedAmount
        decimal MaxPercentOfSalary
        string CustomFormula
    }

    class Expense {
        int Id
        int EmployeeId
        string Description
        decimal Amount
        DateTime Date
    }

    enum ValidationMode {
        FixedAmount
        PercentOfSalary
        CustomFormula
    }
}

namespace Validation {
    interface IValidationStrategy {
        +bool IsValid(Employee employee, Expense expense, out string error)
    }

    class FixedAmountValidation
    class PercentOfSalaryValidation
    class CustomFormulaValidation
    class ValidationStrategyFactory
}

namespace Export {
    abstract class ExpenseExporterTemplate {
        +ExportResult Export(employees, expenses, outputPath)
        #string Serialize(expenses, employees)
    }
    class XmlExpenseExporter
    class JsonExpenseExporter
    class CsvExpenseExporter
    class ExportResult
}

Models.Employee --> Validation.ValidationMode
Validation.FixedAmountValidation ..|> Validation.IValidationStrategy
Validation.PercentOfSalaryValidation ..|> Validation.IValidationStrategy
Validation.CustomFormulaValidation ..|> Validation.IValidationStrategy

Export.ExpenseExporterTemplate --> Validation.IValidationStrategy : uses
Export.XmlExpenseExporter --|> Export.ExpenseExporterTemplate
Export.JsonExpenseExporter --|> Export.ExpenseExporterTemplate
Export.CsvExpenseExporter --|> Export.ExpenseExporterTemplate

Validation.ValidationStrategyFactory --> Validation.IValidationStrategy
@enduml

6. Примерни тестови данни

Служители:

ID	Име					Позиция			Заплата	Вид валидация	Параметър
1	Иван Иванов			Мениджър		4000	PercentOfSalary	30%
2	Петър Петров		Разработчик		2500	FixedAmount		300
3	Мария Георгиева		Стажант			1200	CustomFormula	AMOUNT <= 0.4 * SALARY

Разходи:

ID	Служител			Описание			Сума	Дата
1	Иван Иванов			Командировка Варна	1000	2025-10-30
2	Петър Петров		Лиценз за софтуер	250		2025-10-29
3	Мария Георгиева		Транспорт			700		2025-10-28

При експортиране:
Първите два разхода са валидни;
Третият (700 > 0.4 * 1200 = 480) е невалиден и се показва като грешка.

7. Примерен резултат (JSON експорт)
[
  {
    "expenseId": 1,
    "employeeId": 1,
    "employeeName": "Иван Иванов",
    "position": "Мениджър",
    "description": "Командировка Варна",
    "amount": 1000,
    "date": "2025-10-30T00:00:00"
  },
  {
    "expenseId": 2,
    "employeeId": 2,
    "employeeName": "Петър Петров",
    "position": "Разработчик",
    "description": "Лиценз за софтуер",
    "amount": 250,
    "date": "2025-10-29T00:00:00"
  }
]

8. Демонстрация на работа
Основни стъпки:
Стартиране на приложението.
Преглед на служителите и техните лимити.
Добавяне на нов разход чрез бутона „Добави разход“.
Избор на формат за експорт (напр. JSON).
Натискане на бутона „Експорт“.
Програмата валидира разходите, показва списък с грешки и създава избрания файл.
