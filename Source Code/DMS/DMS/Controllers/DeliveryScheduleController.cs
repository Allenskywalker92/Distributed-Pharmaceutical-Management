using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS.DAL;
using DMS.Service;

namespace DMS.Controllers
{
    public class DeliveryScheduleController : Controller
    {
        //
        // GET: /DeliverySchedule/
        private UnitOfWork unitOfWork = new UnitOfWork();
        KMeans kmeans = new KMeans();
        public ActionResult List()
        {
            var deliverySchedule = unitOfWork.DeliveryScheduleRepository.GetAll().Where(b => b.Status != (int)Status.StatusEnum.Deleted);
            return View(deliverySchedule);
        }
        [HttpPost]
        public ActionResult UpdateStatus(int deliveryScheduleID)
        {
            var deliverySchedule = unitOfWork.DeliveryScheduleRepository.GetByID(deliveryScheduleID);
            var listDeliveryScheduleDetails = deliverySchedule.DeliveryScheduleDetails;
            deliverySchedule.Status = (int)Status.StatusEnum.Complete;
            var drugOrder = new DrugOrder();
            for (int i = 0; i < listDeliveryScheduleDetails.Count; i++)
            {
                var item = listDeliveryScheduleDetails.ElementAt(i);
                drugOrder = new DrugOrder();
                drugOrder = unitOfWork.DrugOrderRepository.GetByID(item.DrugOrderID);
                var drugstore = drugOrder.Drugstore;
                drugOrder.Status = (int)Status.StatusEnum.Complete;
                unitOfWork.DrugOrderRepository.Update(drugOrder);
                unitOfWork.DrugOrderRepository.SaveChanges();
                var payment = new Payment();
                var paymentHistory = unitOfWork.PaymentRepository.Get(b => b.DrugstoreID == drugOrder.DrugstoreID);
                if (paymentHistory.Count()==0)
                {
                    payment.DrugstoreID = drugOrder.DrugstoreID;
                    payment.Amount = drugOrder.TotalPrice;
                    payment.Balance = 0-drugOrder.TotalPrice;
                    drugstore.Debt = 0-drugOrder.TotalPrice;
                }
                else
                {
                    payment.DrugstoreID = drugOrder.DrugstoreID;
                    payment.Amount = drugOrder.TotalPrice;
                    payment.Balance = paymentHistory.Last().Balance - drugOrder.TotalPrice;
                    drugstore.Debt =drugstore.Debt- drugOrder.TotalPrice;
                }
                payment.FullName = drugstore.Account.AccountProfile.FullName;
                payment.PhoneNumber = drugstore.Account.AccountProfile.Phone;
                payment.Date = DateTime.Now;
                payment.PaymentType = true;
                payment.IsActive = true;
                unitOfWork.DrugStoreRepository.Update(drugstore);
                unitOfWork.DrugStoreRepository.SaveChanges();
                unitOfWork.PaymentRepository.Insert(payment);
                unitOfWork.PaymentRepository.SaveChanges();
            }

            unitOfWork.DeliveryScheduleRepository.Update(deliverySchedule);
            unitOfWork.DeliveryScheduleRepository.SaveChanges();
            return null;
        }
        public ActionResult CompleteItem(int deliveryDetailID)
        {
            var deliveryDetails = unitOfWork.DeliveryScheduleDetailsRepository.GetByID(deliveryDetailID);
            deliveryDetails.Status = (int)Status.StatusEnum.Complete;
            var drugOrder = deliveryDetails.DrugOrder;
            var drugstore = drugOrder.Drugstore;
            drugOrder.Status = (int)Status.StatusEnum.Complete;
            unitOfWork.DrugOrderRepository.Update(drugOrder);
            unitOfWork.DrugOrderRepository.SaveChanges();
            var payment = new Payment();
            var paymentHistory = unitOfWork.PaymentRepository.Get(b => b.DrugstoreID == drugOrder.DrugstoreID);
            if (paymentHistory.Count() == 0)
            {
                payment.DrugstoreID = drugOrder.DrugstoreID;
                payment.Amount = drugOrder.TotalPrice;
                payment.Balance = 0 - drugOrder.TotalPrice;
                drugstore.Debt = 0 - drugOrder.TotalPrice;
            }
            else
            {
                payment.DrugstoreID = drugOrder.DrugstoreID;
                payment.Amount = drugOrder.TotalPrice;
                payment.Balance = paymentHistory.Last().Balance - drugOrder.TotalPrice;
                drugstore.Debt = drugstore.Debt - drugOrder.TotalPrice;
            }
            payment.Date = DateTime.Now;
            payment.PaymentType = true;
            payment.IsActive = true;
            unitOfWork.DrugStoreRepository.Update(drugstore);
            unitOfWork.DrugStoreRepository.SaveChanges();
            unitOfWork.PaymentRepository.Insert(payment);
            unitOfWork.PaymentRepository.SaveChanges();
            var deliverySchedule = deliveryDetails.DeliverySchedule;
            if (deliverySchedule.DeliveryScheduleDetails.Count(b => b.Status==(int)Status.StatusEnum.Inprogress)>0)
            {
                return RedirectToAction("ScheduleDetail", new { scheduleID = deliverySchedule.DeliveryScheduleID });
            }
            else
            {
                deliverySchedule.Status = (int) Status.StatusEnum.Complete;
                unitOfWork.DeliveryScheduleRepository.SaveChanges();
                return RedirectToAction("List");
            }
            
        }
        public ActionResult DeleteItem(int deliveryDetailID)
        {
            var deliveryDetails = unitOfWork.DeliveryScheduleDetailsRepository.GetByID(deliveryDetailID);
            deliveryDetails.Status = (int)Status.StatusEnum.Deleted;
            var drugOrder = deliveryDetails.DrugOrder;
            var drugstore = drugOrder.Drugstore;
            drugOrder.Status = (int)Status.StatusEnum.Deleted;
            unitOfWork.DrugOrderRepository.Update(drugOrder);
            unitOfWork.DrugOrderRepository.SaveChanges();
            unitOfWork.DrugStoreRepository.Update(drugstore);
            unitOfWork.DrugStoreRepository.SaveChanges();
            var deliverySchedule = deliveryDetails.DeliverySchedule;
            if (deliverySchedule.DeliveryScheduleDetails.Count(b => b.Status == (int)Status.StatusEnum.Inprogress) > 0)
            {
                return RedirectToAction("ScheduleDetail", new { scheduleID = deliverySchedule.DeliveryScheduleID});
            }
            else
            {
                deliverySchedule.Status = (int)Status.StatusEnum.Complete;
                unitOfWork.DeliveryScheduleRepository.SaveChanges();
                return RedirectToAction("List");
            }

        }
        [HttpPost]
        public ActionResult DeleteSchedule(int deliveryScheduleID)
        {
            var deliverySchedule = unitOfWork.DeliveryScheduleRepository.GetByID(deliveryScheduleID);
            var listDeliveryScheduleDetails = deliverySchedule.DeliveryScheduleDetails;
            var drugOrder = new DrugOrder();
            for (int i = listDeliveryScheduleDetails.Count - 1; i >= 0; i--)
            {
                var item = listDeliveryScheduleDetails.ElementAt(i);
                drugOrder = new DrugOrder();
                drugOrder = unitOfWork.DrugOrderRepository.GetByID(item.DrugOrderID);
                drugOrder.Status = (int)Status.StatusEnum.Approved;
                unitOfWork.DrugOrderRepository.Update(drugOrder);
                unitOfWork.DrugOrderRepository.SaveChanges();
            }
            deliverySchedule.Status = (int)Status.StatusEnum.Deleted;
            unitOfWork.DeliveryScheduleRepository.Update(deliverySchedule);
            unitOfWork.DeliveryScheduleRepository.SaveChanges();
            unitOfWork.DeliveryScheduleDetailsRepository.SaveChanges();
            return null;
        }
        public ActionResult CreateSchedule()
        {
            ViewBag.DeliveryManID = new SelectList(unitOfWork.DeliverymanRepository.GetAll(), "DeliveryManID", "FullName");
            Session["Order"] = unitOfWork.DrugOrderRepository.Get(b => b.Status == (int)Status.StatusEnum.Approved);
            return View();
        }
        [HttpPost]
        public ActionResult CreateSchedule(string deliveryManID)
        {
            List<int> listOrderID = new List<int>();
            if (Session["ListOrderID"] != null)
            {
                var schedule = new DeliverySchedule();
                listOrderID = (List<int>)Session["ListOrderID"];
                schedule.DeliveryMan = unitOfWork.DeliverymanRepository.GetByID(int.Parse(deliveryManID));
                //schedule.DueDate = DateTime.Parse(dueDate);
                schedule.CreateDate = DateTime.Now;
                var scheduleDetail = new DeliveryScheduleDetail();
                var order = new DrugOrder();
                Point startPoint = new Point();
                startPoint.x = 10.7972388;
                startPoint.y = 106.6803467;
                List<Point> listOrder = new List<Point>();
                for (int i = 0; i < listOrderID.Count; i++)
                {
                    Point drugOrderPoint = new Point();
                    drugOrderPoint.DrugOrder = unitOfWork.DrugOrderRepository.GetByID(listOrderID[i]);
                    var drugstoreCoordinate = drugOrderPoint.DrugOrder.Drugstore.Coordinate;
                    string[] drugstoreCoordinates = drugstoreCoordinate.Split(',');
                    drugOrderPoint.x = double.Parse(drugstoreCoordinates[0]);
                    drugOrderPoint.y = double.Parse(drugstoreCoordinates[1]);
                    listOrder.Add(drugOrderPoint);
                }
                var tempPoint = new Point();
                List<Point> resuledList = new List<Point>();
                bool flag = true;
                tempPoint = startPoint;
                for (int i = listOrder.Count - 1; i >= 0; i--)
                {
                        if (listOrder.Count > 0)
                        {
                            tempPoint = NearestPoint(tempPoint, listOrder);
                            resuledList.Add(tempPoint);
                            listOrder.Remove(tempPoint);
                        }
                }
                for (int i = 0; i < resuledList.Count; i++)
                {
                    scheduleDetail = new DeliveryScheduleDetail();
                    order = new DrugOrder();
                    scheduleDetail.DrugOrderID = resuledList[i].DrugOrder.DrugOrderID;
                    scheduleDetail.Status =(int) Status.StatusEnum.Inprogress;
                    order = resuledList[i].DrugOrder;
                    order.Status = (int)Status.StatusEnum.Inprogress;
                    unitOfWork.DrugOrderRepository.Update(order);
                    schedule.DeliveryScheduleDetails.Add(scheduleDetail);
                }
                schedule.Status = (int)Status.StatusEnum.Inprogress;
                bool check = unitOfWork.DeliveryScheduleRepository.Insert(schedule);
                if (check)
                {
                    Session["ListOrderID"] = null;
                }
                unitOfWork.DeliveryScheduleRepository.SaveChanges();
                unitOfWork.DeliveryScheduleDetailsRepository.SaveChanges();
                unitOfWork.DrugOrderRepository.SaveChanges();
            }

            return Json(listOrderID);
        }

        [HttpPost]
        public ActionResult UpdateSessionOrder(int orderID)
        {
            if (Session["ListOrderID"] != null)
            {
                bool flag = true;
                var listOrderID = (List<int>)Session["ListOrderID"];
                for (int i = 0; i < listOrderID.Count; i++)
                {
                    if (orderID == listOrderID[i])
                    {
                        listOrderID.RemoveAt(i);
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    listOrderID.Add(orderID);
                }
                Session["ListOrderID"] = listOrderID;
            }
            else
            {
                List<int> listOrderID = new List<int>();
                listOrderID.Add(orderID);
                Session["ListOrderID"] = listOrderID;
            }
            return new EmptyResult();
        }


        public ActionResult ScheduleDetail(int scheduleID)
        {
            var schedule = unitOfWork.DeliveryScheduleRepository.GetByID(scheduleID);
            return View(schedule);
        }

        public ActionResult PrintScheduleDetail(int scheduleID)
        {
            var schedule = unitOfWork.DeliveryScheduleRepository.GetByID(scheduleID);
            return View(schedule);
        }


        public class Point
        {
            public double x { get; set; }
            public double y { get; set; }
            public DrugOrder DrugOrder { get; set; }
            public double distance { get; set; }
        }
        public Point NearestPoint(Point startPoint, List<Point> listPoint)
        {
            List<Point> resultList = new List<Point>();
            for (int i = 0; i < listPoint.Count; i++)
            {
                listPoint[i].distance = kmeans.GetDistance(startPoint.x, startPoint.y, listPoint[i].x, listPoint[i].y);
                resultList.Add(listPoint[i]);
            }
            var resultedPoint = new Point();
            var tempPoint = new Point();
            tempPoint.distance = double.MaxValue;
            for (int i = 0; i < resultList.Count; i++)
            {
                if (tempPoint.distance > resultList[i].distance)
                {
                    tempPoint = resultList[i];
                }
            }
            //resultList = BubbleSort(resultList);
            return tempPoint;
        }
        //public double FindDistance(Point startPoint, Point destinationPoint)
        //{
        //    double x = Math.Pow(startPoint.x - destinationPoint.x, 2);
        //    double y = Math.Pow(startPoint.y - destinationPoint.y, 2);
        //    double distance = Math.Sqrt(x + y);
        //    return distance;
        //}
        //public List<Point> BubbleSort(List<Point> list)
        //{
        //    bool flag = true;
        //    Point temp;
        //    for (int i = 1; (i < list.Count) && flag; i++)
        //    {
        //        flag = false;
        //        for (int j = 0; j < list.Count - 1; j++)
        //        {
        //            if (list[j + 1].distance < list[j].distance)
        //            {
        //                temp = list[j];
        //                list[j] = list[j + 1];
        //                list[j + 1] = temp;
        //                flag = true;
        //            }
        //        }
        //    }
        //    return list;
        //}
    }
}
