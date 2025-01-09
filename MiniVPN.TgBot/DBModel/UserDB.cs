using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserChecker.Server.Model;

public class UserDB
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("tgId")]
    public long TgId { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("actualVpnCode")]
    public string ActualVpnCode { get; set; }
    
    [JsonPropertyName("role")]
    public RoleDB roleDb { get; set; }
    
    [JsonPropertyName("paidUntil")]
    public DateTime PaidUntil { get; set; }

    public UserDB() { }

    public UserDB(long id, long tgId, string name, string actualCode, int r, DateTime paidUntil)
    {
        Id = id;
        TgId = tgId;
        Name = name;
        ActualVpnCode = actualCode;
        roleDb = r == 1? RoleDB.Admin : RoleDB.User;
        PaidUntil = paidUntil;
    }

    public UserDB(UserRequestDTO userDTO)
    {
        TgId = userDTO.TgId;
        Name = userDTO.Name;
        ActualVpnCode = userDTO.ActualVpnCode;
        PaidUntil = userDTO.PaidUntil;
    }
}