using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Globalization;

using sb_admin_2.Web1.Models.Mapping;

namespace sb_admin_2.Web1.Models
{
    public class InvoiceList : List<Invoice>
    {

    }

    public class InvoiceStatus
    {
        private enum InvoiceStatusEnum { none, offered, booked, sold, voided };

        private Dictionary<InvoiceStatusEnum, string> DescriptionMapping;

        private InvoiceStatusEnum _Status;

        public InvoiceStatus()
        {
            DescriptionMapping = new Dictionary<InvoiceStatusEnum, string>();

            DescriptionMapping.Add(InvoiceStatusEnum.offered, "Offer");
            DescriptionMapping.Add(InvoiceStatusEnum.booked, "Booked");
            DescriptionMapping.Add(InvoiceStatusEnum.sold, "Sold");
            DescriptionMapping.Add(InvoiceStatusEnum.voided, "Void");

            _Status = InvoiceStatusEnum.none;
        }

        public void SetStatus(string val)
        {
            foreach (var item in DescriptionMapping)
            {
                if (item.Value == val)
                {
                    _Status = item.Key;
                    return;
                }
            }
            throw new ArgumentException("Unknown invoice status");
        }

        public string GetStatus()
        {
            string str = string.Empty;
            if (DescriptionMapping.TryGetValue(_Status, out str))
                return str;
            else
                throw new ArgumentException("Unhandled state of Invoice status");
        }
    }

    public class ServiceType
    {
        private enum ServiceTypeEnum { none, aviaticket, hotel, insurance, trainticket }

        private Dictionary<ServiceTypeEnum, string> DescriptionMapping;

        private ServiceTypeEnum _State;

        public ServiceType()
        {
            DescriptionMapping = new Dictionary<ServiceTypeEnum, string>();

            DescriptionMapping.Add(ServiceTypeEnum.aviaticket, "Avia ticket");
            DescriptionMapping.Add(ServiceTypeEnum.hotel, "Hotel");
            DescriptionMapping.Add(ServiceTypeEnum.insurance, "Insurance");
            DescriptionMapping.Add(ServiceTypeEnum.trainticket, "Train ticket");

            _State = ServiceTypeEnum.none;
        }

        public void SetStatus(string val)
        {
            foreach (var item in DescriptionMapping)
            {
                if (item.Value == val)
                {
                    _State = item.Key;
                    return;
                }
            }
            throw new ArgumentException("Unknown service type");
        }

        public string GetStatus()
        {
            string str = string.Empty;
            if (DescriptionMapping.TryGetValue(_State, out str))
                return str;
            else
                throw new ArgumentException("Unhandled service type");
        }
    }

    public abstract class Invoice
    {
        private InvoiceStatus invoiceStatus;

        protected ServiceType serviceType;

        protected abstract string GetPath();

        protected abstract void SetPath(string val);

        protected abstract string GetDate();

        protected abstract string SetDate(string val);
        

        public int ID { get; set; }

        public string Status
        {
            get
            {
                return invoiceStatus.GetStatus();
            }
            set
            {
                invoiceStatus.SetStatus(value);
            }
        }

        public string Service
        {
            get
            {
                return serviceType.GetStatus();
            }
        }

        public string Description { get; set; }

        public Money Amount { get; set; }

        public Person Client { get; set; }

        public Person ResponsiblePerson { get; set; }

        public string Path
        {
            get { return GetPath(); }
            set { SetPath(value); }
        }

        public string Date 
        {
            get { return GetDate(); }
            set { SetDate(value); }
        }

        public string ServiceProvider { get; set; }
        

        public Invoice() 
        {
            Amount = new Money();
            invoiceStatus = new InvoiceStatus();
            serviceType = new ServiceType();
            Client = new Person();
            ResponsiblePerson = new Person();
        }

        public void CreateTestData()
        {
            
        }
    }

    public class AviaTicketInvoice : Invoice
    {
        private enum TripType { oneway, round };


        private TripType triptype;


        protected override string GetPath()
        {
            switch (triptype)
            {
                case TripType.oneway:
                    {
                        return StartPoint + " - " + FinalDestination;
                    }
                case TripType.round:
                    {
                        return StartPoint + " - " + FinalDestination + ", " + FinalDestination + " - " + StartPoint;
                    }
                default:
                    return string.Empty;
            }
        }

        protected override void SetPath(string val)
        {
            throw new NotImplementedException("SetPath for avia ticket should be implemented");
        }

        protected override string GetDate()
        {
            switch (triptype)
            {
                case TripType.oneway:
                    {
                        return DepartureStr;
                    }
                case TripType.round:
                    {
                        return DepartureStr + " - " + DepartureBackStr;
                    }
                default:
                    return string.Empty;
            }
        }

        protected override string SetDate(string val)
        {
            throw new NotImplementedException("SetDate for avia ticket should be implemented");
        }


        public AviaTicketInvoice()
        {
            triptype = TripType.oneway;
            serviceType.SetStatus("Avia ticket");
        }


        public bool IsRoundTrip
        {
            get
            {
                return triptype == TripType.round;
            }
            set
            {
                if (value)
                    triptype = TripType.round;
                else
                    triptype = TripType.oneway;
            }
        }

        public DateTime Departure { get; set; }

        public DateTime DepartureBack { get; set; }

        public string StartPoint { get; set; }

        public string FinalDestination { get; set; }


        public string DepartureStr 
        {
            get { return Departure.ToString("ddMMMyy", Preferences.cultureInfo); }
            set { throw new NotImplementedException("DepatureStr for avia ticket is not implemented"); }
        }

        public string DepartureBackStr 
        {
            get { return DepartureBack.ToString("ddMMMyy", Preferences.cultureInfo); }
            set { throw new NotImplementedException("DepatureStr for avia ticket is not implemented"); }
        }
    }

    public class HotelInvoice : Invoice
    {
        protected override string GetPath()
        {
            throw new NotImplementedException("GetPath for hotel should be implemented");
        }

        protected override void SetPath(string val)
        {
            throw new NotImplementedException("SetPath for hotel should be implemented");
        }

        protected override string GetDate()
        {
            throw new NotImplementedException("GetDate for hotel should be implemented");
        }

        protected override string SetDate(string val)
        {
            throw new NotImplementedException("SetDate for hotel should be implemented");
        }


        public HotelInvoice()
        {
            serviceType.SetStatus("Hotel");
        }
    }

    public class InsuranceInvoice : Invoice
    {
        protected override string GetPath()
        {
            throw new NotImplementedException("GetPath for insurance should be implemented");
        }

        protected override void SetPath(string val)
        {
            throw new NotImplementedException("SetPath for insurance should be implemented");
        }

        protected override string GetDate()
        {
            throw new NotImplementedException("GetDate for insurance should be implemented");
        }

        protected override string SetDate(string val)
        {
            throw new NotImplementedException("SetDate for insurance should be implemented");
        }


        public InsuranceInvoice()
        {
            serviceType.SetStatus("Insurance");
        }
    }
}