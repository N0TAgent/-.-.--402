using System;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

List<Order> repo = [
    new Order(1, new(2023,1,1),"����", "������������","�������","������� ��������� ����������", "89630348648", "� ��������"),
    new Order(2, new(2024,1,3),"����", "������������","�������","������� ��������� ����������", "89630348648", "� ��������"),
    new Order(3, new(2023,1,4),"����", "������������","�������","������� ��������� ����������", "89630348648", "� ��������"),
    new Order(4, new(2022,7,12),"����", "������������","�������","������� ��������� ����������", "89630348648", "� ��������"),
    new Order(5, new(2023, 2, 10), "����", "����������������", "�����", "������ ������ ������������", "89261234567", "� ��������"),
    new Order(3, new(2023, 3, 15), "�����", "������������", "�������", "��������� ����� ��������", "89561237890", "� �������� �������"),
    new Order(4, new(2023, 4, 25), "����", "����������������", "�����", "�������� ������� �������", "89312345678", "� �������� �������"),
    new Order(5, new(2023, 5, 30), "����", "������������", "����������", "���������� ������ �������������", "89451234567", "� ��������"),
    new Order(6, new(2023, 6, 12), "����", "����������������", "�����", "������ ���� ���������", "89761234567", "� ��������"),
    new Order(7, new(2023, 7, 20), "����", "������������", "�������", "�������� ���� ������������", "89991234567", "� �������� �������"),
    new Order(8, new(2023, 8, 18), "�����", "����������������", "�����", "������� ������ ��������", "89284561234", "� �������� �������"),
    new Order(9, new(2023, 9, 5), "����", "������������", "����������", "������� ������� ����������", "89637890123", "� �������� �������"),
    new Order(10, new(2023, 10, 10), "����", "����������������", "�����", "��������� �������� ����������", "89064578901", "� �������� �������"),
    new Order(11, new(2023, 11, 25), "�����", "������������", "�������", "������ ������� ��������", "89474561234", "� �������� �������"),
    ];

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

string message = "";


app.MapGet("/orders", (int param = 0) =>
{
    string buffer = message;
    message = "";
    if (param != 0)
        return new { repo = repo.FindAll(x => x.Number == param), message = buffer };
    return new { repo, message = buffer };
});


app.MapGet("create", ([AsParameters] Order dto) => repo.Add(dto));


app.MapGet("update", ([AsParameters] OrderUpdateDTO dto) =>
{
    var o = repo.Find(x => x.Number == dto.Number);
    if (o == null)
        return;
    if (dto.Status != o.Status && dto.Status != "")
    {
        o.Status = dto.Status;
        message += $"������ ������ ����� {o.Number} ������\n";
        if(o.Status == "������ � ������")
        {
            message += $"������ ����� {o.Number} ������ � ������";
            o.EndDate = DateOnly.FromDateTime(DateTime.Now);
        }
    }
    if (dto.ProblemDescription != "")
        o.ProblemDescription = dto.ProblemDescription;
    if (dto.Master != "")
        o.Master = dto.Master;
    if (dto.Comment != "")
        o.Comments.Add(dto.Comment);
});

int complete_count() => repo.FindAll(x => x.Status == "������ � ������").Count;

Dictionary<string, int> get_problem_type_stat() =>
    repo.GroupBy(x => x.ProblemDescription).Select(x => (x.Key, x.Count()))
    .ToDictionary(k => k.Key, v => v.Item2);


double get_average_time_to_complete() =>
    complete_count() == 0 ? 0 : repo.FindAll(x => x.Status == "������ � ������")
    .Select(x => x.EndDate.Value.DayNumber - x.StartDate.DayNumber)
    .Sum() / complete_count();


app.MapGet("/statistics", () => new
{
    complete_count = complete_count(),
    problem_type_stat = get_problem_type_stat(),
    average_time_to_complete = get_average_time_to_complete()

});

app.Run();



class Order (int number, DateOnly startDate, string typeTexnick, string model, string problemDescription, string fioClient, string phoneNumber, string status)
{
    public int Number { get; set; } = number;
    public DateOnly StartDate { get; set; } = startDate;
    public DateOnly? EndDate { get; set; } = null;
    public string TypeTexnick { get; set; } = typeTexnick;
    public string Model { get; set; } = model;
    public string ProblemDescription { get; set; } = problemDescription;
    public string FioClient { get; set;} = fioClient;
    public string PhoneNumber { get; set; } = phoneNumber;
    public string Status { get; set; } = status;
    public string Master { get; set; } = "�� ��������";
    public List<string>? Comments { get; set; } = [];

}

record class OrderUpdateDTO(int Number, string? Status = "", string? ProblemDescription = "", string? Master = "", string? Comment = "");
