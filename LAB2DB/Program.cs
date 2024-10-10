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
                    Console.WriteLine("Выберите задачу:");
                    Console.WriteLine("1. Выборка всех данных из таблицы 'Клиенты' (3.2.1)");
                    Console.WriteLine("2. Выборка клиентов с фильтрацией по скидке > 10% (3.2.2)");
                    Console.WriteLine("3. Максимальная стоимость услуг по сотрудникам (3.2.3)");
                    Console.WriteLine("4. Выборка клиентов и их услуг (3.2.4)");
                    Console.WriteLine("5. Поиск услуг сотрудников по конкретной дате (3.2.5)");
                    Console.WriteLine("6. Добавить нового клиента (3.2.6)");
                    Console.WriteLine("7. Добавить новую услугу (3.2.7)");
                    Console.WriteLine("8. Удалить клиента (3.2.8)");
                    Console.WriteLine("9. Удалить выполненную услугу (3.2.9)");
                    Console.WriteLine("10. Обновить скидку клиента (3.2.10)");
                    Console.WriteLine("0. Выход");

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
                            Console.WriteLine("Неверный выбор. Пожалуйста, выберите заново.");
                            break;
                    }
                }
            }
        }

        // 3.2.1. Выборка всех данных из таблицы "Клиенты"
        static void GetAllClients(Db8331Context context)
        {
            var clients = context.Clients.ToList();
            Console.WriteLine("Все клиенты:");
            foreach (var client in clients)
            {
                Console.WriteLine($"{client.Id}: {client.FullName}");
            }
        }

        // 3.2.2. Выборка клиентов с фильтрацией по скидке > 10%
        static void GetFilteredClients(Db8331Context context)
        {
            var filteredClients = context.Clients
                                         .Where(c => c.Discount > 10)
                                         .ToList();
            Console.WriteLine("Клиенты с скидкой больше 10%:");
            foreach (var client in filteredClients)
            {
                Console.WriteLine($"{client.Id}: {client.FullName}, Скидка: {client.Discount}");
            }
        }


        // 3.2.3. Максимальная стоимость услуг по сотрудникам
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

            Console.WriteLine("Максимальная стоимость услуг по сотрудникам:");
            foreach (var result in maxCostPerEmployee)
            {
                Console.WriteLine($"Сотрудник: {result.EmployeeId}, Max стоимость: {result.MaxCost}");
            }
        }

        // 3.2.4. Выборка клиентов и их услуг
        static void GetClientsWithServices(Db8331Context context)
        {
            var clientsWithServices = context.Clients
                                             .Select(c => new
                                             {
                                                 ClientName = c.FullName,
                                                 Services = c.PerformedServices.Select(s => s.Service.Name)
                                             })
                                             .ToList();

            Console.WriteLine("Клиенты и их услуги:");
            foreach (var client in clientsWithServices)
            {
                Console.WriteLine($"Клиент: {client.ClientName}, Услуги: {string.Join(", ", client.Services)}");
            }
        }

        // 3.2.5. Поиск услуг сотрудников по конкретной дате
        static void GetServicesByDate(Db8331Context context)
        {
            Console.WriteLine("Введите дату для поиска (гггг-мм-дд): ");
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

            Console.WriteLine($"Услуги сотрудников за дату {inputDate}:");
            foreach (var item in filteredEmployeesWithServices)
            {
                Console.WriteLine($"Сотрудник: {item.EmployeeName}, Услуга: {item.ServiceName}, Дата: {item.ServiceDate}");
            }
        }

        // 3.2.6. Добавить нового клиента
        static void AddNewClient(Db8331Context context)
        {
            Console.WriteLine("Введите полное имя клиента:");
            var fullName = Console.ReadLine();

            Console.WriteLine("Введите адрес клиента:");
            var address = Console.ReadLine();

            Console.WriteLine("Введите телефон клиента:");
            var phone = Console.ReadLine();

            Console.WriteLine("Введите скидку клиента:");
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
            Console.WriteLine("Новый клиент добавлен.");
        }


        // 3.2.7. Добавить новую услугу
        static void AddNewPerformedService(Db8331Context context)
        {
            Console.WriteLine("Введите ID клиента:");
            var clientId = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите ID сотрудника:");
            var employeeId = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите ID услуги:");
            var serviceId = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите дату выполнения услуги (гггг-мм-дд):");
            var serviceDate = DateOnly.Parse(Console.ReadLine());

            Console.WriteLine("Введите стоимость услуги:");
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
            Console.WriteLine("Новая услуга добавлена.");
        }

        // 3.2.8. Удалить клиента
        static void DeleteClient(Db8331Context context)
        {
            Console.WriteLine("Введите ID клиента для удаления:");
            var clientId = int.Parse(Console.ReadLine());

            var clientToRemove = context.Clients.Find(clientId);
            if (clientToRemove != null)
            {
                context.Clients.Remove(clientToRemove);
                context.SaveChanges();
                Console.WriteLine("Клиент удален.");
            }
            else
            {
                Console.WriteLine("Клиент не найден.");
            }
        }

        // 3.2.9. Удалить выполненную услугу
        static void DeletePerformedService(Db8331Context context)
        {
            Console.WriteLine("Введите ID услуги для удаления:");
            var serviceId = int.Parse(Console.ReadLine());

            var serviceToRemove = context.PerformedServices.Find(serviceId);
            if (serviceToRemove != null)
            {
                context.PerformedServices.Remove(serviceToRemove);
                context.SaveChanges();
                Console.WriteLine("Услуга удалена.");
            }
            else
            {
                Console.WriteLine("Услуга не найдена.");
            }
        }

        // 3.2.10. Обновить скидку клиента
        static void UpdateClientDiscount(Db8331Context context)
        {
            Console.WriteLine("Введите ID клиента для обновления скидки:");
            var clientId = int.Parse(Console.ReadLine());

            var clientToUpdate = context.Clients.FirstOrDefault(c => c.Id == clientId);
            if (clientToUpdate != null)
            {
                Console.WriteLine("Введите новую скидку для клиента:");
                clientToUpdate.Discount = decimal.Parse(Console.ReadLine());
                context.SaveChanges();
                Console.WriteLine("Скидка клиента обновлена.");
            }
            else
            {
                Console.WriteLine("Клиент не найден.");
            }
        }
    }
}
