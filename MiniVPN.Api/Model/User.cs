using System.ComponentModel.DataAnnotations;

namespace UserChecker.Server.Model;

public class User
{
    [Key]
    public long Id { get; set; }

    public long TgId { get; set; }
    public string Name { get; set; }
    public string ActualVpnCode { get; set; }
    public Role Role { get; set; } = 0;
    public DateTime PaidUntil { get; set; }

    public User() { }

    public User(UserRequestDTO userDTO)
    {
        Id = 0;
        TgId = userDTO.TgId;
        Name = userDTO.Name;
        ActualVpnCode = userDTO.ActualVpnCode;
        PaidUntil = userDTO.PaidUntil;
    }
}