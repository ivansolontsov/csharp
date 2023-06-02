using System.ComponentModel.DataAnnotations;

namespace Test2.Extensions.common.enums;

public enum FriendActions
{
    [Display(Name = "Добавить в друзья")]
    Add = 0,
    [Display(Name = "Отклонить заявку")]
    Decline = 1,
}