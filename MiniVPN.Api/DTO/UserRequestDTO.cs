namespace UserChecker.Server.Model;

public class UserRequestDTO(long tgId, string name, DateTime paidUntil, string actualVpnCode = "")
{
    public long TgId { get; set; } = tgId;
    public string Name { get; set; } = name;
    public string ActualVpnCode { get; set; } = actualVpnCode;
    public DateTime PaidUntil { get; set; } = paidUntil;
}