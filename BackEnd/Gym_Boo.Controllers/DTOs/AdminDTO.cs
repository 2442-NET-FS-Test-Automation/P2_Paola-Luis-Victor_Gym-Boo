namespace Gym_Boo.Controllers.Administration;

//Controlls DTO
public record DisciplineDto(string Name);
public record CreateInstructorDto(string FirstName, string LastName, string Email, string Password);