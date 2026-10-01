namespace MyTasks.Domain;

public enum MyTaskItemStatus
{
    Todo=1,
    InProgress=2,}
public enum MyTaskPriority
{
    Low=1,
    Medium=2,
    High =3
}
public record MyTaskItemModel
{
   public Guid Id{get;init;}=Guid.NewGuid();
   public string Title{get;init;}=string.Empty;
   public string Description{get;init;}=string.Empty;
   public MyTaskItemStatus Status{get;init;}=MyTaskItemStatus.InProgress;
   public MyTaskPriority priority{get; init;}=MyTaskPriority.High;
   public DateTime? DueDate{get;init;}
   public Guid? CategoryId{get;init;}
   public Guid? AssignedUserId{get;init;}
   public DateTime CreatedAt{get;init;}=DateTime.UtcNow;
   public DateTime? UpdatedAt{get;init;}
}