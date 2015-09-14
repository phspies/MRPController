using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Threading;

public class App
{
    public const int LOGON32_LOGON_INTERACTIVE = 2;
    public const int LOGON32_LOGON_SERVICE = 3;
    public const int LOGON32_PROVIDER_DEFAULT = 0;

    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    public static extern bool LogonUser(
        String lpszUserName,
        String lpszDomain,
        String lpszPassword,
        int dwLogonType,
        int dwLogonProvider,
        ref IntPtr phToken);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public extern static bool CloseHandle(IntPtr handle);
    public class InstanceCounters
    {
        public string instance { get; set; }
        public double value { get; set; }
        public CounterSample s0 { get; set; }
        public CounterSample s1 { get; set; }

        public DateTime timestamp { get; set; }
    }
    public class PerfCounter
    {
        public string category { get; set; }
        public string counter { get; set; }
        public List<InstanceCounters> instances { get; set; }

    }
    public static List<PerfCounter> _countertree = new List<PerfCounter>();
    private static List<String> _categories = new List<string>();
    private static String machinename = @"FSPIES-T420S";

    public static void Main()
    {
        IntPtr userHandle = new IntPtr(0);
        LogonUser("Phillip.Spies", "NA", "ZAQ!2wsx", LOGON32_LOGON_SERVICE, LOGON32_PROVIDER_DEFAULT, ref userHandle);
        WindowsIdentity identity = new WindowsIdentity(userHandle);
        WindowsImpersonationContext context = identity.Impersonate();
        _categories.Add("Processor");
        _categories.Add("Processor Information");
        _categories.Add("Memory");
        _categories.Add("LogicalDisk");
        _categories.Add("PhysicalDisk");
        _categories.Add("Network Interface");
        _categories.Add("Server");
        _categories.Add("Double-Take Connection");
        _categories.Add("Double-Take Kernel");
        _categories.Add("Double-Take Source");
        _categories.Add("Double-Take Target");

        machinename = Environment.MachineName;

        while (true)
        {
            foreach (string pcc in _categories)
            {
                try
                {
                    PerformanceCounterCategory _pc = new PerformanceCounterCategory(pcc, machinename);
                    //_countertree.Add(new _PerfCounter() { category = pcc, counter = "" });
                    if (_pc.CategoryType == PerformanceCounterCategoryType.SingleInstance)
                    {
                        List<InstanceCounters> _instances = new List<InstanceCounters>();
                        _instances.Add(new InstanceCounters() { instance = "" });
                        foreach (var _counter in _pc.GetCounters())
                        {
                            if (!_countertree.Exists(x => x.category == pcc && x.counter == _counter.CounterName))
                            {
                                _countertree.Add(new PerfCounter() { category = pcc, counter = _counter.CounterName, instances = _instances });
                            }
                            PerformanceCounter _pcounter = new PerformanceCounter(_counter.CategoryName, _counter.CounterName, string.Empty, machinename);
                            InstanceCounters _counterobject = _countertree.Where(
                                x => x.category == _counter.CategoryName &&
                                x.counter == _counter.CounterName)
                                .Select(x => x.instances.SingleOrDefault(y => y.instance == ""))
                                .FirstOrDefault();
                            _counterobject.s1 = _pcounter.NextSample();
                            if (_counterobject.s0.CounterType != _counterobject.s1.CounterType)
                            {
                                _counterobject.s0 = _counter.NextSample();
                            }
                            _counterobject.value = CounterSampleCalculator.ComputeCounterValue(_counterobject.s0, _counterobject.s1);
                            _counterobject.s0 = _counterobject.s1;
                            Console.WriteLine(String.Format("{0}   {1}:{2}:{3} = {4}", _counterobject.timestamp, _pcounter.CategoryName, _pcounter.CounterName, _counterobject.instance, _counterobject.value));
                        }
                    }
                    else
                    {
                        List<InstanceCounters> _instances = new List<InstanceCounters>();
                        foreach (string _instance in _pc.GetInstanceNames())
                        {
                            if (!_instances.Exists(x => x.instance == _instance))
                            {
                                _instances.Add(new InstanceCounters() { instance = _instance });
                            }
                        }
                        foreach (var _instance in _instances)
                        {
                            foreach (var _counter in _pc.GetCounters(_instance.instance))
                            {
                                if (!_countertree.Exists(x => x.category == pcc && x.counter == _counter.CounterName))
                                {
                                    _countertree.Add(new PerfCounter() { category = pcc, counter = _counter.CounterName, instances = _instances });
                                }
                                else
                                {
                                    if (!_countertree.Find(x => x.category == pcc && x.counter == _counter.CounterName).instances.Exists(x => x.instance == _instance.instance))
                                    {
                                        _countertree.Find(x => x.category == pcc && x.counter == _counter.CounterName).instances.Add(_instance);
                                    }
                                }
                                PerformanceCounter _pcounter = new PerformanceCounter(_counter.CategoryName, _counter.CounterName, _counter.InstanceName, machinename);

                                InstanceCounters _counterobject = _countertree.Where(
                                    x => x.category == _counter.CategoryName &&
                                    x.counter == _counter.CounterName)
                                    .Select(x => x.instances.SingleOrDefault(y => y.instance == _counter.InstanceName))
                                    .FirstOrDefault();
                                _counterobject.s1 = _pcounter.NextSample();
                                if (_counterobject.s0.CounterType != _counterobject.s1.CounterType)
                                {
                                    _counterobject.s0 = _counter.NextSample();
                                }
                                _counterobject.value = CounterSampleCalculator.ComputeCounterValue(_counterobject.s0, _counterobject.s1);
                                _counterobject.s0 = _counterobject.s1;
                                Console.WriteLine(String.Format("{0}   {1}:{2}:{3} = {4}", _counterobject.timestamp, _pcounter.CategoryName, _pcounter.CounterName, _counterobject.instance, _counterobject.value));

                            }
                        }


                    }
                }
                catch (System.InvalidOperationException error)
                {
                    Console.WriteLine(error.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            Thread.Sleep(new TimeSpan(0,5,0));




            //    foreach (PerfCounter _category in _countertree)
            //    {
            //        foreach (InstanceCounters _instance in _category.instances)
            //        {
            //            PerformanceCounter _pcounter = null;
            //            if (new PerformanceCounterCategory(_category.category, machinename).CategoryType == PerformanceCounterCategoryType.MultiInstance)
            //            {
            //                _pcounter = new PerformanceCounter(_category.category, _category.counter, _instance.instance, machinename);
            //            }
            //            else if ((new PerformanceCounterCategory(_category.category, machinename).CategoryType == PerformanceCounterCategoryType.longInstance))
            //            {
            //                _pcounter = new PerformanceCounter(_category.category, _category.counter, string.Empty, machinename);

            //            }else
            //            {
            //                Console.WriteLine("Someting is wrong!@!!");
            //            }
            //            InstanceCounters _counterobject = _countertree.Where(
            //                x => x.category == _pcounter.CategoryName &&
            //                x.counter == _pcounter.CounterName)
            //                .Select(x => x.instances.longOrDefault(y => y.instance == _pcounter.InstanceName))
            //                .FirstOrDefault();

            //            if (first)
            //            {
            //                _counter.s0 = _pcounter.NextSample();
            //                _counter.value = CounterSampleCalculator.ComputeCounterValue(_counter.s0, _pcounter.NextSample());
            //                _counter.s1 = _pcounter.NextSample();

            //            }
            //            else
            //            {

            //                _counter.s0 = _counter.s1;
            //                _counter.value = CounterSampleCalculator.ComputeCounterValue(_counter.s0, _pcounter.NextSample());
            //                _counter.s1 = _pcounter.NextSample();

            //            }

            //            //var dt = new DateTime(_pcounter.NextSample().TimeStamp100nSec);

            //            //_instance.timestamp = dt;
            //            //switch (_pcounter.CounterType.ToString())
            //            //{
            //            //    case "Timer100NsInverse":
            //            //        _instance.lastvalue = Time100Inverse(_instance);
            //            //        break;
            //            //    case "NumberOfItems64":
            //            //        _instance.lastvalue = NumberOfItems64(_instance);
            //            //        break;
            //            //    case "NumberOfItems32":
            //            //        _instance.lastvalue = NumberOfItems32(_instance);
            //            //        break;
            //            //    case "RawFraction":
            //            //        _instance.lastvalue = RawFraction(_instance);
            //            //        break;
            //            //    case "RateOfCountsPerSecond64":
            //            //        _instance.lastvalue = RateOfCountsPerSecond64(_instance);
            //            //        break;
            //            //    case "Timer100Ns":
            //            //        _instance.lastvalue = Timer100Ns(_instance);
            //            //        break;
            //            //    case "RateOfCountsPerSecond32":
            //            //        _instance.lastvalue = RateOfCountsPerSecond32(_instance);
            //            //        break;
            //            //    case "AverageCount64":
            //            //        _instance.lastvalue = AverageCount64(_instance);
            //            //        break;
            //            //    case "AverageTimer32":
            //            //        _instance.lastvalue = AverageTimer32(_instance);
            //            //        break;
            //            //    case "5571840":
            //            //        _instance.lastvalue = PERF_COUNTER_100NS_QUEUELEN_TYPE(_instance);
            //            //        break;
            //            //    case "542573824":
            //            //        _instance.lastvalue = PERF_PRECISION_100NS_TIMER(_instance);
            //            //        break;
            //            //    default:
            //            //        Console.WriteLine(String.Format("{0} not found", _pcounter.CounterType));
            //            //        break;
            //            //}
            //            Console.WriteLine(String.Format("{0}   {1}:{2}:{3} = {4}", _instance.timestamp, _category.category, _category.counter, _instance.instance, _instance.value));
            //        }

            //    }
            //    //Thread.Sleep(5000);
            //    first = false;

            //}
            ////PERF_COUNTER_100NS_QUEUELEN_TYPE = 5571840,
            ////PERF_PRECISION_100NS_TIMER = 542573824,
        }
    }

    //private static long RateOfCountsPerSecond32(InstanceCounters _historycounter)
    //{
    //    long numerator = (long)(_historycounter.s1.RawValue - _historycounter.s0.RawValue);
    //    long denomenator = (long)(_historycounter.s1.TimeStamp - _historycounter.s0.TimeStamp) / (long)_historycounter.s1.SystemFrequency;
    //    long value = numerator / denomenator;
    //    return value < 0 ? 0 : value;

        
    //}
    //private static long Timer100Ns(InstanceCounters _historycounter)
    //{
    //    // (N 1 - N 0) / (D 1 - D 0) x 100
    //    long numerator = (long)_historycounter.s1.RawValue - (long)_historycounter.s0.RawValue;
    //    long denomenator = (long)_historycounter.s1.TimeStamp100nSec - (long)_historycounter.s0.TimeStamp100nSec;
    //    long value = ((((long)numerator / (long)denomenator))) * 100;
    //    return value < 0 ? 0 : value;
    //}
    //private static long RateOfCountsPerSecond64(InstanceCounters _historycounter)
    //{
    //    long numerator = (long)(_historycounter.s1.RawValue - _historycounter.s0.RawValue);
    //    long denomenator = (long)(_historycounter.s1.TimeStamp - _historycounter.s0.TimeStamp) / (long)_historycounter.s1.SystemFrequency;
    //    long value = numerator / denomenator;
    //    return value < 0 ? 0 : value;
    //}
    //private static long RawFraction(InstanceCounters _historycounter)
    //{
    //    // (N 0 / D 0) x 100
    //    long numerator = (long)_historycounter.s1.RawValue;
    //    long denomenator = (long)_historycounter.s1.BaseValue;
    //    long value = (((long)numerator / (long)denomenator)) * 100;
    //    return value < 0 ? 0 : value;
    //}
    //private static long Time100Inverse(InstanceCounters _historycounter)
    //{
    //    //(1 - ((N 1 - N 0) / (D 1 - D 0))) x 100
    //    long numerator = (long)_historycounter.s1.RawValue - (long)_historycounter.s0.RawValue;
    //    long denomenator = (long)_historycounter.s1.TimeStamp100nSec - (long)_historycounter.s0.TimeStamp100nSec;
    //    long value = (1 - (((long)numerator / (long)denomenator))) * 100;
    //    return value < 0 ? 0 : value;

    //}
    //private static long NumberOfItems64(InstanceCounters _historycounter)
    //{
    //    return _historycounter.s1.RawValue;
    //}
    //private static long NumberOfItems32(InstanceCounters _historycounter)
    //{
    //    return _historycounter.s1.RawValue;
    //}
    //private static long AverageCount64(InstanceCounters _historycounter)
    //{
    //    long numerator = (long)_historycounter.s1.RawValue - (long)_historycounter.s0.RawValue;
    //    long denomenator = (long)_historycounter.s1.BaseValue - (long)_historycounter.s0.BaseValue;
    //    long counterValue = numerator / denomenator;
    //    return (counterValue);
    //}
    //private static long AverageTimer32(InstanceCounters _historycounter)
    //{
    //    Int64 n1 = _historycounter.s1.RawValue;
    //    Int64 n0 = _historycounter.s0.RawValue;
    //    ulong f = (ulong)_historycounter.s1.SystemFrequency;
    //    Int64 d1 = _historycounter.s1.BaseValue;
    //    Int64 d0 = _historycounter.s0.BaseValue;

    //    long numerator = (long)(n1 - n0);
    //    long denominator = (long)(d1 - d0);
    //    long counterValue = (long)((numerator / f) / denominator);
    //    return (counterValue);
    //}
    //private static long PERF_COUNTER_100NS_QUEUELEN_TYPE(InstanceCounters _historycounter)
    //{

    //    long n = (long)_historycounter.s1.RawValue - (long)_historycounter.s0.RawValue;
    //    long d = (long)_historycounter.s1.TimeStamp100nSec - (long)_historycounter.s0.TimeStamp100nSec;
    //    long counterValue = (d == 0 ? 0 : (n / d));
    //    return (counterValue);

    //}
    //private static long PERF_PRECISION_100NS_TIMER(InstanceCounters _historycounter)
    //{
    //    ulong n = (ulong)_historycounter.s1.RawValue - (ulong)_historycounter.s0.RawValue;
    //    ulong d = (ulong)_historycounter.s1.TimeStamp100nSec - (ulong)_historycounter.s0.TimeStamp100nSec;
    //    long counterValue = (d == 0 ? 0 : ((n / d) * 100));
    //    return (counterValue);

    //}
    //Formula: (N 1 - N 0) / (D 1 - D 0) x 100, 
    //    where the numerator represents the portions of the sample interval during which 
    //    the monitored components were active, and the denominator represents the total elapsed time of the sample interval.
}

