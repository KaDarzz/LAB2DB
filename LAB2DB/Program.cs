using System;
using System.Linq;
using LAB2DB.Models;

namespace LAB2DB
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new Db8331Context())
            {
                while (true)
                {
                    Console.WriteLine("�������� ������:");
                    Console.WriteLine("1. ������� ���� ������ �� ������� '�������' (3.2.1)");
                    Console.WriteLine("2. ������� �������� � ����������� �� ������ > 10% (3.2.2)");
                    Console.WriteLine("3. ������������ ��������� ����� �� ����������� (3.2.3)");
                    Console.WriteLine("4. ������� �������� � �� ����� (3.2.4)");
                    Console.WriteLine("5. ����� ����� ����������� �� ���������� ���� (3.2.5)");
                    Console.WriteLine("6. �������� ������ ������� (3.2.6)");
                    Console.WriteLine("7. �������� ����� ������ (3.2.7)");
                    Console.WriteLine("8. ������� ������� (3.2.8)");
                    Console.WriteLine("9. ������� ����������� ������ (3.2.9)");
                    Console.WriteLine("10. �������� ������ ������� (3.2.10)");
                    Console.WriteLine("0. �����");

                    var choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            GetAllClients(context);
                            break;
                        case "2":
                            GetFilteredClients(context);
                            break;
                        case "3":
                            GetMaxServiceCostByEmployee(context);
                            break;
                        case "4":
                            GetClientsWithServices(context);
                            break;
                        case "5":
                            GetServicesByDate(context);
                            break;
                        case "6":
                            AddNewClient(context);
                            break;
                        case "7":
                            AddNewPerformedService(context);
                            break;
                        case "8":
                            DeleteClient(context);
                            break;
                        case "9":
                            DeletePerformedService(context);
                            break;
                        case "10":
                            UpdateClientDiscount(context);
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("�������� �����. ����������, �������� ������.");
                            break;
                    }
                }
            }
        }

        // 3.2.1. ������� ���� ������ �� ������� "�������"
        static void GetAllClients(Db8331Context context)
        {
            var clients = context.Clients.ToList();
            Console.WriteLine("��� �������:");
            foreach (var client in clients)
            {
                Console.WriteLine($"{client.Id}: {client.FullName}");
            }
        }

        // 3.2.2. ������� �������� � ����������� �� ������ > 10%
        static void GetFilteredClients(Db8331Context context)
        {
            var filteredClients = context.Clients
                                         .Where(c => c.Discount > 10)
                                         .ToList();
            Console.WriteLine("������� � ������� ������ 10%:");
            foreach (var client in filteredClients)
            {
                Console.WriteLine($"{client.Id}: {client.FullName}, ������: {client.Discount}");
            }
        }


        // 3.2.3. ������������ ��������� ����� �� �����������
        static void GetMaxServiceCostByEmployee(Db8331Context context)
        {
            var maxCostPerEmployee = context.PerformedServices
                                            .GroupBy(p => p.EmployeeId)
                                            .Select(g => new
                                            {
                                                EmployeeId = g.Key,
                                                MaxCost = g.Max(p => p.Cost)
                                            })
                                            .ToList();

            Console.WriteLine("������������ ��������� ����� �� �����������:");
            foreach (var result in maxCostPerEmployee)
            {
                Console.WriteLine($"���������: {result.EmployeeId}, Max ���������: {result.MaxCost}");
            }
        }

        // 3.2.4. ������� �������� � �� �����
        static void GetClientsWithServices(Db8331Context context)
        {
            var clientsWithServices = context.Clients
                                             .Select(c => new
                                             {
                                                 ClientName = c.FullName,
                                                 Services = c.PerformedServices.Select(s => s.Service.Name)
                                             })
                                             .ToList();

            Console.WriteLine("������� � �� ������:");
            foreach (var client in clientsWithServices)
            {
                Console.WriteLine($"������: {client.ClientName}, ������: {string.Join(", ", client.Services)}");
            }
        }

        // 3.2.5. ����� ����� ����������� �� ���������� ����
        static void GetServicesByDate(Db8331Context context)
        {
            Console.WriteLine("������� ���� ��� ������ (����-��-��): ");
            var inputDate = DateOnly.Parse(Console.ReadLine());

            var filteredEmployeesWithServices = context.PerformedServices
                                                       .Where(p => p.ServiceDate == inputDate)
                                                       .Select(p => new
                                                       {
                                                           EmployeeName = p.Employee.FullName,
                                                           ServiceName = p.Service.Name,
                                                           ServiceDate = p.ServiceDate
                                                       })
                                                       .ToList();

            Console.WriteLine($"������ ����������� �� ���� {inputDate}:");
            foreach (var item in filteredEmployeesWithServices)
            {
                Console.WriteLine($"���������: {item.EmployeeName}, ������: {item.ServiceName}, ����: {item.ServiceDate}");
            }
        }

        // 3.2.6. �������� ������ �������
        static void AddNewClient(Db8331Context context)
        {
            Console.WriteLine("������� ������ ��� �������:");
            var fullName = Console.ReadLine();

            Console.WriteLine("������� ����� �������:");
            var address = Console.ReadLine();

            Console.WriteLine("������� ������� �������:");
            var phone = Console.ReadLine();

            Console.WriteLine("������� ������ �������:");
            var discount = decimal.Parse(Console.ReadLine());

            var newClient = new Client
            {
                FullName = fullName,
                Address = address,
                Phone = phone,
                Discount = discount,
                TotalServicesCost = 0
            };

            context.Clients.Add(newClient);
            context.SaveChanges();
            Console.WriteLine("����� ������ ��������.");
        }


        // 3.2.7. �������� ����� ������
        static void AddNewPerformedService(Db8331Context context)
        {
            Console.WriteLine("������� ID �������:");
            var clientId = int.Parse(Console.ReadLine());

            Console.WriteLine("������� ID ����������:");
            var employeeId = int.Parse(Console.ReadLine());

            Console.WriteLine("������� ID ������:");
            var serviceId = int.Parse(Console.ReadLine());

            Console.WriteLine("������� ���� ���������� ������ (����-��-��):");
            var serviceDate = DateOnly.Parse(Console.ReadLine());

            Console.WriteLine("������� ��������� ������:");
            var cost = decimal.Parse(Console.ReadLine());

            var newService = new PerformedService
            {
                ClientId = clientId,
                EmployeeId = employeeId,
                ServiceId = serviceId,
                ServiceDate = serviceDate,
                Cost = cost
            };

            context.PerformedServices.Add(newService);
            context.SaveChanges();
            Console.WriteLine("����� ������ ���������.");
        }

        // 3.2.8. ������� �������
        static void DeleteClient(Db8331Context context)
        {
            Console.WriteLine("������� ID ������� ��� ��������:");
            var clientId = int.Parse(Console.ReadLine());

            var clientToRemove = context.Clients.Find(clientId);
            if (clientToRemove != null)
            {
                context.Clients.Remove(clientToRemove);
                context.SaveChanges();
                Console.WriteLine("������ ������.");
            }
            else
            {
                Console.WriteLine("������ �� ������.");
            }
        }

        // 3.2.9. ������� ����������� ������
        static void DeletePerformedService(Db8331Context context)
        {
            Console.WriteLine("������� ID ������ ��� ��������:");
            var serviceId = int.Parse(Console.ReadLine());

            var serviceToRemove = context.PerformedServices.Find(serviceId);
            if (serviceToRemove != null)
            {
                context.PerformedServices.Remove(serviceToRemove);
                context.SaveChanges();
                Console.WriteLine("������ �������.");
            }
            else
            {
                Console.WriteLine("������ �� �������.");
            }
        }

        // 3.2.10. �������� ������ �������
        static void UpdateClientDiscount(Db8331Context context)
        {
            Console.WriteLine("������� ID ������� ��� ���������� ������:");
            var clientId = int.Parse(Console.ReadLine());

            var clientToUpdate = context.Clients.FirstOrDefault(c => c.Id == clientId);
            if (clientToUpdate != null)
            {
                Console.WriteLine("������� ����� ������ ��� �������:");
                clientToUpdate.Discount = decimal.Parse(Console.ReadLine());
                context.SaveChanges();
                Console.WriteLine("������ ������� ���������.");
            }
            else
            {
                Console.WriteLine("������ �� ������.");
            }
        }
    }
}
