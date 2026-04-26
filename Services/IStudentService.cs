using kitucxa.Models;

namespace kitucxa.Service
{
    public interface IStudentService
    {
        // thao tác của admin
        List<Student> GetAll();
        Student? GetById(int id);
        void Add(Student student);
        void Update(Student student);
        void Delete(int id);

        // thao tác của sinh viên
        StudentDashboardVm? GetDashboard(int studentId);

        
    }
}