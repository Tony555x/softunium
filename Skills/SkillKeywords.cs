namespace Harduni.Skills;

public static class SkillKeywords
{
    public static string GetExplanation(string keyword)
    {
        return keyword.ToLower() switch
        {
            "poison" or "отрова" => "Отрова: Нанася щети всеки ход. Стаковете намаляват с 1/3 всеки път (мин. 1).",
            "percent" or "процент" => "% бонуси за статистики/щети се събират: +50% Атака + +50% Атака = +100% Атака (не +125%).",
            "negative" => "Негативни % използват деление: -100% = /2, -200% = /3, -300% = /4.",
            _ => ""
        };
    }
}
