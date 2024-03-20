// See https://aka.ms/new-console-template for more information

var random = new Random();
const int maxCustomers = 4;
const int numChairs = 3;
var allDone = false;

var waitingRoom = new Semaphore(numChairs, numChairs);
var barberChair = new Semaphore(1, 1);
var barberSleepChair = new Semaphore(0, 1);
var seatBelt = new Semaphore(0, 1);

void GetBarberActions()
{
    while (!allDone)
    {
        Console.WriteLine("The barber is sleeping...");
        barberSleepChair.WaitOne();

        if (!allDone)
        {
            Console.WriteLine("The barber cuts...");
            Thread.Sleep(random.Next(1, 3) * 1000);
            Console.WriteLine("The barber cut client hair!");
            seatBelt.Release();
        }
        else
        {
            Console.WriteLine("The barber is sleeping...");
        }
    }
    return;
}

void GetCustomerActions(object number)
{
    int Number = (int)number;
    Console.WriteLine("Client {0} goes to the barber...", Number);

    Thread.Sleep(random.Next(1, 5) * 1000);

    Console.WriteLine("Client {0} has arrived!", Number);
    waitingRoom.WaitOne();
    Console.WriteLine("Client {0} enters the waiting room...", Number);

    barberChair.WaitOne();
    waitingRoom.Release();
    Console.WriteLine("Client {0} wakes up the barber...", Number);

    barberSleepChair.Release();
    seatBelt.WaitOne();
    barberChair.Release();
    Console.WriteLine("Client {0} leaves the barber...", Number);
}

var barberThread = new Thread(GetBarberActions);
barberThread.Start();

var customers = new List<Thread>();

for (int i = 0; i < maxCustomers; i++)
{
    var customer = new Thread(GetCustomerActions);
    customer.Start(i);
    customers.Add(customer);
}

foreach (var customer in customers)
{
    customer.Join();
}

allDone = true;
barberSleepChair.Release();
barberThread.Join();

Console.WriteLine("End of work!");
Console.ReadLine();
