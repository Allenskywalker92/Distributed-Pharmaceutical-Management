using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DMS.DAL;
using DMS.Service;

namespace DMS.Controllers
{
    public class DrugstoreController : Controller
    {
        //
        // GET: /Drugstore/
        private UnitOfWork unitOfWork = new UnitOfWork();
        KMeans kMeans = new KMeans();
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult AssignAccountToSaleMan()
        //{
        //    AssignSaleMan();
        //    return RedirectToAction("ListNewRegister", "Account");
        //}
        //public ActionResult AssignDrugstoreoSaleMan()
        //{
        //    AssignSaleMan();
        //    return RedirectToAction("ListNewRegister", "Account");
        //}
        public ActionResult UpdateSessionGroup(int groupID)
        {
            if (Session["ListGroupID"] != null)
            {
                bool flag = true;
                var listOrderID = (List<int>)Session["ListGroupID"];
                for (int i = 0; i < listOrderID.Count; i++)
                {
                    if (groupID == listOrderID[i])
                    {
                        listOrderID.RemoveAt(i);
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    listOrderID.Add(groupID);
                }
                Session["ListGroupID"] = listOrderID;
            }
            else
            {
                List<int> listOrderID = new List<int>();
                listOrderID.Add(groupID);
                Session["ListGroupID"] = listOrderID;
            }
            return new EmptyResult();
        }
        //public ActionResult DeleteDrugstoreGroup()
        //{
        //    if (Session["ListGroupID"] != null)
        //    {
        //        bool flag = true;
        //        var listGroupID = (List<int>)Session["ListGroupID"];
        //        var listGroup = new List<int>();
        //        listGroup.AddRange(listGroupID);
        //        for (int i = listGroupID.Count - 1; i >= 0; i--)
        //        {
        //            var id = Int32.Parse(listGroupID[i].ToString());
        //            var drusgtore = unitOfWork.DrugStoreRepository.Get(b => b.DrugstoreGroupID == id).ToList();
        //            for (int j = 0; j < drusgtore.Count; j++)
        //            {
        //                drusgtore[j].DrugstoreGroupID = null;
        //                unitOfWork.DrugStoreRepository.Update(drusgtore[j]);
        //                unitOfWork.DrugStoreRepository.SaveChanges();
        //            }
        //            unitOfWork.DrugstoreGroupRepository.Delete(listGroupID[i]);
        //            unitOfWork.DrugstoreGroupRepository.SaveChanges();
        //            listGroupID.RemoveAt(i);
        //        }
        //        Session["ListGroupID"] = listGroupID;
        //        return Json(listGroup);

        //    }
        //    return new EmptyResult();
        //}   


        //public void AssignSaleMan()
        //{
        //    ////var accountList =
        //    ////    unitOfWork.AccountRepository.Get(
        //    ////        b => b.Drugstore.SalesmanID == null && b.RoleID == 4 && b.IsPending == true && b.IsActive == true);
        //    //List<Drugstore> listDrugstore = (List<Drugstore>)unitOfWork.DrugStoreRepository.Get(b => b.SalesmanID == null&&b.OwnerID!=null);
        //    ////List<Drugstore> listDrugstore = (List<Drugstore>) unitOfWork.AccountRepository.Get(
        //    ////    b => b.Drugstore.SalesmanID == null && b.RoleID == 4 && b.IsPending == true && b.IsActive == true)
        //    ////    .Select(b => b.Drugstore);
        //    //var listSalesman = unitOfWork.AccountRepository.Get(b=>b.RoleID==3);
        //    //Random r = new Random();
        //    //int index = 0;
        //    //if (listSalesman!=null)
        //    //{
        //    //    var listClusterQuantity = ListClusterQuantity(listSalesman.Count(), listDrugstore.Count);
        //    //    foreach (int i in Enumerable.Range(0, listSalesman.Count()).OrderBy(x => r.Next()))
        //    //    {
        //    //        var drugstores = NearestDrugstores(listDrugstore, listSalesman.ElementAt(i), listClusterQuantity[index]);
        //    //        for (int j = 0; j < drugstores.Count; j++)
        //    //        {
        //    //            Drugstore drugstore = new Drugstore();
        //    //            drugstore = drugstores[j];
        //    //            drugstore.SalesmanID = listSalesman.ElementAt(i).AccountID;
        //    //            unitOfWork.DrugStoreRepository.Update(drugstore);
        //    //            unitOfWork.DrugStoreRepository.SaveChanges();
        //    //        }
        //    //        for (int j = listDrugstore.Count - 1; j >= 0; j--)
        //    //        {
        //    //            for (int k = 0; k < drugstores.Count; k++)
        //    //            {
        //    //                if (listDrugstore[j].DrugstoreID == drugstores[k].DrugstoreID)
        //    //                {
        //    //                    listDrugstore.RemoveAt(j);
        //    //                    break;
        //    //                }
        //    //            }

        //    //        }
        //    //        index++;
        //    //    }
        //    //}

        //    var listSalemans = unitOfWork.AccountRepository.GetAll().Where(a => a.RoleID == 3).ToList();
        //    var allGroups = kMeans.Processing();
        //    var groupDrugstore = unitOfWork.DrugstoreGroupRepository.GetAll().ToList();
        //    if (allGroups!=null)
        //    {
        //        if (groupDrugstore.Count >0)
        //        {
        //            for (int i = 0; i < listSalemans.Count; i++)
        //            {
        //                var index = -1;
        //                double minDistance = double.MaxValue;
        //                for (int j = 0; j < allGroups.Count; j++)
        //                {
        //                    double curDistance = DistanceFromSalesman(allGroups[j].Item2, listSalemans[i]);
        //                    if (curDistance < minDistance)
        //                    {
        //                        minDistance = curDistance;
        //                        index = j;
        //                    }
        //                }
        //                var drugstoreGroup = new DrugstoreGroup();
        //                drugstoreGroup.SalesmanID = listSalemans[i].AccountID;
        //                drugstoreGroup.DrugstoreCentroid = allGroups[index].Item2;
        //                unitOfWork.DrugstoreGroupRepository.Insert(drugstoreGroup);
        //                unitOfWork.DrugstoreGroupRepository.SaveChanges();

        //                for (int k = 0; k < allGroups[index].Item1.Count; k++)
        //                {
        //                    var drugstoreID = allGroups[index].Item1[k].DrugstoreID;
        //                    var drugstore =
        //                        unitOfWork.DrugStoreRepository.GetAll()
        //                            .Where(s => s.DrugstoreID == drugstoreID)
        //                            .Single();
        //                    drugstore.DrugstoreGroupID = drugstoreGroup.DrugstoreGroupID;
        //                    unitOfWork.DrugStoreRepository.Update(drugstore);
        //                    unitOfWork.DrugStoreRepository.SaveChanges();
        //                }
        //                allGroups.Remove(allGroups[index]);
        //            }
        //        }
        //        else
        //        {
        //           for (int i = 0; i < groupDrugstore.Count(); i++)
        //            {
        //                var index = -1;
        //                double minDistance = double.MaxValue;
        //                for (int j = 0; j < allGroups.Count; j++)
        //                {
        //                    double curDistance = DistanceFromCentroid(allGroups[j].Item2, groupDrugstore[i]);
        //                    if (curDistance < minDistance)
        //                    {
        //                        minDistance = curDistance;
        //                        index = j;
        //                    }
        //                }
        //                //var drugstoreGroup = new DrugstoreGroup();
        //                //drugstoreGroup.SalesmanID = listSalemans[i].AccountID;
        //                //drugstoreGroup.DrugstoreCentroid = allGroups[index].Item2.Coordinate;
        //                //unitOfWork.DrugstoreGroupRepository.Insert(drugstoreGroup);
        //                //unitOfWork.DrugstoreGroupRepository.SaveChanges();

        //                for (int k = 0; k < allGroups[index].Item1.Count; k++)
        //                {
        //                    var drugstoreID = allGroups[index].Item1[k].DrugstoreID;
        //                    var drugstore =
        //                        unitOfWork.DrugStoreRepository.GetAll()
        //                            .Where(s => s.DrugstoreID == drugstoreID)
        //                            .Single();
        //                    drugstore.DrugstoreGroupID = groupDrugstore[i].DrugstoreGroupID;
        //                    unitOfWork.DrugStoreRepository.Update(drugstore);
        //                    unitOfWork.DrugStoreRepository.SaveChanges();
        //                }
        //                var listDrugstore =
        //                    unitOfWork.DrugStoreRepository.Get(
        //                        b => b.DrugstoreGroupID == groupDrugstore[i].DrugstoreGroupID).ToList();
        //                var centroid = kMeans.FindCentroid(listDrugstore);
        //                allGroups.Remove(allGroups[index]);
        //            }
        //            if (allGroups.Count>0)
        //            {
        //                var saleman = unitOfWork.AccountRepository.Get(b => b.RoleID == 3);
        //                var salemandID = unitOfWork.DrugstoreGroupRepository.GetAll().Select(b => b.SalesmanID);
        //                var salemanNew = (from i in saleman
        //                    where !salemandID.Contains(i.AccountID)
        //                    select i).ToList();
        //                for (int i = 0; i < salemanNew.Count; i++)
        //                {
        //                    var index = -1;
        //                    double minDistance = double.MaxValue;
        //                    for (int j = 0; j < allGroups.Count; j++)
        //                    {
        //                        double curDistance = DistanceFromSalesman(allGroups[j].Item2, salemanNew[i]);
        //                        if (curDistance < minDistance)
        //                        {
        //                            minDistance = curDistance;
        //                            index = j;
        //                        }
        //                    }
        //                    var drugstoreGroup = new DrugstoreGroup();
        //                    drugstoreGroup.SalesmanID = salemanNew[i].AccountID;
        //                    drugstoreGroup.DrugstoreCentroid = allGroups[index].Item2;
        //                    unitOfWork.DrugstoreGroupRepository.Insert(drugstoreGroup);
        //                    unitOfWork.DrugstoreGroupRepository.SaveChanges();

        //                    for (int k = 0; k < allGroups[index].Item1.Count; k++)
        //                    {
        //                        var drugstoreID = allGroups[index].Item1[k].DrugstoreID;
        //                        var drugstore =
        //                            unitOfWork.DrugStoreRepository.GetAll()
        //                                .Where(s => s.DrugstoreID == drugstoreID)
        //                                .Single();
        //                        drugstore.DrugstoreGroupID = drugstoreGroup.DrugstoreGroupID;
        //                        unitOfWork.DrugStoreRepository.Update(drugstore);
        //                        unitOfWork.DrugStoreRepository.SaveChanges();
        //                    }
        //                    allGroups.Remove(allGroups[index]);
        //                }
        //            }
        //        }
        //    }
            
         

            
            
        //}

        public double DistanceFromSalesman(string CentroidCoordinate, Account account)
        {
            //KMeans kmeans=new KMeans();
            var pointA=new DMS.Service.KMeans.Point2D();
            var pointB=new DMS.Service.KMeans.Point2D();

            string[] tempString = account.AccountProfile.Coordinate.Split(',');
            //List<double> coodinates = new List<double>();
            //for (int i = 0; i < tempString.Count(); i++)
            //{
            //    coodinates.Add(double.Parse(tempString[i]));
            //}
            pointA.x=double.Parse(tempString[0]);
            pointA.y=double.Parse(tempString[1]);
            List<double> result = new List<double>();
            string[] temp = new string[2];
            temp = CentroidCoordinate.Split(',');
            pointB.x=double.Parse(temp[0]);
            pointB.y=double.Parse(temp[1]);
            //result.Add(double.Parse(temp[0]));
            //result.Add(double.Parse(temp[1]));
            
            //var poin3D = kMeans.Change(drugStore);
            //var point3D = kMeans.ChangeCoordinateMapToCoordinate3D(result[0], result[1]);
            return kMeans.GetDistance(pointA, pointB);
            //return kMeans.GetDistance(point3D, kMeans.ChangeCoordinateMapToCoordinate3D(coodinates[0], coodinates[1]));
        }
        //public double DistanceFromCentroid(string CentroidCoordinate, DrugstoreGroup drugstoreGroup)
        //{
        //    string[] tempString = drugstoreGroup.DrugstoreCentroid.Split(',');
        //    List<double> coodinates = new List<double>();
        //    for (int i = 0; i < tempString.Count(); i++)
        //    {
        //        coodinates.Add(double.Parse(tempString[i]));
        //    }
        //    List<double> result = new List<double>();
        //    string[] temp = new string[2];
        //    temp = CentroidCoordinate.Split(',');
        //    result.Add(double.Parse(temp[0]));
        //    result.Add(double.Parse(temp[1]));
        //    //var poin3D = kMeans.Change(drugStore);
        //    var point3D = kMeans.ChangeCoordinateMapToCoordinate3D(result[0], result[1]);
        //    return kMeans.GetDistance(point3D, kMeans.ChangeCoordinateMapToCoordinate3D(coodinates[0], coodinates[1]));
        //    //return kMeans.GetDistance(kMeans.Change(drugStore), kMeans.ChangeCoordinateMapToCoordinate3D(coodinates[0], coodinates[1]));
        //}

        public List<Drugstore> NearestDrugstores(List<Drugstore> listDrugstore, Account saleman, int count)
        {
            string coordinate = saleman.AccountProfile.Coordinate;
            string[] stringSalemanCoordinates = coordinate.Split(',');
            List<double> salemanCoordinate = new List<double>();
            for (int i = 0; i < stringSalemanCoordinates.Count(); i++)
            {
                salemanCoordinate.Add(double.Parse(stringSalemanCoordinates[i]));
            }
            List<Drugstore> nearestDrugstores = new List<Drugstore>(count);
            double x = 0;
            double y = 0;
            List<Point> listPoint = new List<Point>();
            for (int i = 0; i < listDrugstore.Count; i++)
            {
                var point = new Point();
                point.Drugstore = listDrugstore[i];
                coordinate = listDrugstore[i].Coordinate;
                string[] stringDrugstoreCoordinates = coordinate.Split(',');
                List<double> drugstoreCoordinate = new List<double>();
                for (int j = 0; j < stringDrugstoreCoordinates.Count(); j++)
                {
                    drugstoreCoordinate.Add(double.Parse(stringDrugstoreCoordinates[j]));
                }
                point.distance = FindDistance(salemanCoordinate, drugstoreCoordinate);
                listPoint.Add(point);
            }
            var resultList = BubbleSort(listPoint);
            for (int i = 0; i < count; i++)
            {
                nearestDrugstores.Add(resultList[i].Drugstore);
            }
            return nearestDrugstores;
        }
        public double FindDistance(List<double> salemanCoordinate, List<double> drugstoreCoordinate)
        {
            double x = Math.Pow(salemanCoordinate[0] - drugstoreCoordinate[0], 2);
            double y = Math.Pow(salemanCoordinate[1] - drugstoreCoordinate[1], 2);
            double distance = Math.Sqrt(x + y);
            return distance;
        }
        public List<int> ListClusterQuantity(int salemanQuantity, int drugstoreQuantity)
        {
            List<int> result = new List<int>();
            int temp = drugstoreQuantity / salemanQuantity;
            for (int i = 0; i < salemanQuantity; i++)
            {
                result.Add(temp);
            }
            int left = drugstoreQuantity % salemanQuantity;
            if (drugstoreQuantity % salemanQuantity != 0)
            {
                for (int i = 0; i < left; i++)
                {
                    result[i] = result[i] + 1;
                }
            }
            return result;
        }
        public List<Point> BubbleSort(List<Point> list)
        {
            bool flag = true;
            Point temp;
            for (int i = 1; (i < list.Count) && flag; i++)
            {
                flag = false;
                for (int j = 0; j < list.Count - 1; j++)
                {
                    if (list[j + 1].distance < list[j].distance)
                    {
                        temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                        flag = true;
                    }
                }
            }
            return list;
        }
    }

    public class Point
    {
        public Drugstore Drugstore { get; set; }
        public double distance { get; set; }
    }

}
