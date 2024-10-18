using System;
using System.Collections.Generic;

public interface IPaymentStrategy
{
    void Pay(decimal amount);
}

public class CreditCardPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата банковской картой на сумму {amount}.");
    }
}

public class PayPalPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата через PayPal на сумму {amount}.");
    }
}

public class CryptoPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата криптовалютой на сумму {amount}.");
    }
}

public class PaymentContext
{
    private IPaymentStrategy _paymentStrategy;

    public void SetPaymentStrategy(IPaymentStrategy paymentStrategy)
    {
        _paymentStrategy = paymentStrategy;
    }

    public void ProcessPayment(decimal amount)
    {
        _paymentStrategy.Pay(amount);
    }
}

public interface IObserver
{
    void Update(string currency, decimal rate);
}

public interface ISubject
{
    void RegisterObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
    void NotifyObservers();
}

public class CurrencyExchange : ISubject
{
    private List<IObserver> _observers = new List<IObserver>();
    private Dictionary<string, decimal> _currencyRates = new Dictionary<string, decimal>();

    public void RegisterObserver(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            foreach (var currency in _currencyRates)
            {
                observer.Update(currency.Key, currency.Value);
            }
        }
    }

    public void SetCurrencyRate(string currency, decimal rate)
    {
        _currencyRates[currency] = rate;
        NotifyObservers();
    }
}

public class Trader : IObserver
{
    private string _name;

    public Trader(string name)
    {
        _name = name;
    }

    public void Update(string currency, decimal rate)
    {
        Console.WriteLine($"Трейдер {_name} получил обновление: {currency} = {rate}");
    }
}

public class TradingRobot : IObserver
{
    public void Update(string currency, decimal rate)
    {
        if (rate > 100)
        {
            Console.WriteLine($"Робот: Покупаю {currency}, курс {rate}");
        }
        else
        {
            Console.WriteLine($"Робот: Продаю {currency}, курс {rate}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        PaymentContext paymentContext = new PaymentContext();

        Console.WriteLine("Выберите способ оплаты:");
        Console.WriteLine("1 - Банковская карта");
        Console.WriteLine("2 - PayPal");
        Console.WriteLine("3 - Криптовалюта");
        int choice = int.Parse(Console.ReadLine());

        switch (choice)
        {
            case 1:
                paymentContext.SetPaymentStrategy(new CreditCardPayment());
                break;
            case 2:
                paymentContext.SetPaymentStrategy(new PayPalPayment());
                break;
            case 3:
                paymentContext.SetPaymentStrategy(new CryptoPayment());
                break;
            default:
                Console.WriteLine("Неверный выбор. Используется банковская карта по умолчанию.");
                paymentContext.SetPaymentStrategy(new CreditCardPayment());
                break;
        }

        Console.WriteLine("Введите сумму для оплаты:");
        decimal amount = decimal.Parse(Console.ReadLine());
        paymentContext.ProcessPayment(amount);

        CurrencyExchange exchange = new CurrencyExchange();
        Trader trader1 = new Trader("Monkey d Alinur");
        Trader trader2 = new Trader("Roronoa Zoro");
        TradingRobot robot = new TradingRobot();

        exchange.RegisterObserver(trader1);
        exchange.RegisterObserver(trader2);
        exchange.RegisterObserver(robot);

        Console.WriteLine("\nОбновление курсов валют:");
        exchange.SetCurrencyRate("USD", 105m);
        exchange.SetCurrencyRate("EUR", 95m);

        exchange.RemoveObserver(trader1);
        Console.WriteLine("\nУдален трейдер Monkey D Alinur и обновлен курс валют:");
        exchange.SetCurrencyRate("USD", 90m);
    }
}
