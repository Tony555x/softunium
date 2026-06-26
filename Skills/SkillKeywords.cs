namespace Harduni.Skills;

public static class SkillKeywords
{
    public static string GetExplanation(string keyword)
    {
        return keyword.ToLower() switch
        {
            "poison" or "отрова" => "Отрова: Нанася щети всеки ход. Стаковете намаляват с 1/3 всеки път (мин. 1).",
            _ => ""
        };
    }
}
