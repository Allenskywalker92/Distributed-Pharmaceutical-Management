using DMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS.Service
{
    public class KMeans
    {
        double earthRadius = 6371;
        public struct Point3D
        {
            public double x;
            public double y;
            public double z;
        };
        public struct Point2D
        {
            public double x;
            public double y;
        };
        UnitOfWork unitOfWork = new UnitOfWork();
        public List<Tuple<List<Drugstore>, string>> Processing()
        {
            List<Drugstore> allStore = unitOfWork.DrugStoreRepository.GetAll().ToList();
            if (allStore.Count!=0)
            {
                int numberOfCluster = unitOfWork.AccountRepository.GetAll().Where(a => a.RoleID == 3&a.IsActive==true).Count();
                return DoKMeans(allStore, numberOfCluster);
            }
            return null;
        }
        //public List<Tuple<List<Drugstore>, string>> Processing(int numberCluster)
        //{
        //    List<Drugstore> allStore = unitOfWork.DrugStoreRepository.Get(b => b.DrugstoreGroup == null).ToList();
        //    if (allStore.Count != 0)
        //    {
        //        //int numberOfCluster = unitOfWork.AccountRepository.GetAll().Where(a => a.RoleID == 3).Count();
        //        return DoKMeans(allStore, numberCluster);
        //    }
        //    return null;
        //}
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        private double rad2deg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }
        private List<double> ChangeToCoordinate(Drugstore point)
        {
            List<double> result = new List<double>();
            string[] temp = new string[2];
            temp = point.Coordinate.Split(',');
            result.Add(double.Parse(temp[0]));
            result.Add(double.Parse(temp[1]));
            return result;
        }
        public Point3D ChangeCoordinateMapToCoordinate3D(double longitude, double latitude)
        {
            //change from degree to radian
            longitude = deg2rad(longitude);
            latitude = deg2rad(latitude);
            //change to 3D coordinate base on the formula
            double x = Math.Cos(latitude) * Math.Cos(longitude) * earthRadius;
            double y = Math.Cos(latitude) * Math.Sin(longitude) * earthRadius;
            double z = Math.Sin(latitude) * earthRadius;
            Point3D result;
            result.x = x;
            result.y = y;
            result.z = z;
            return result;
        }
        private string ChangeCoordinate3DToCoordinateMap(Point3D point){
            double lat = (float)Math.Asin(point.z / earthRadius);
            double lon = (float)Math.Atan(point.y / point.x);
            double latitude = rad2deg(lat);
            double longitude = rad2deg(lon);
            return (longitude + "," + latitude);
        }
        private string ChangeCoordinate2DToCoordinateMap(Point2D point)
        {
            double latitude = point.x;
            double longitude = point.y;
            return (longitude + "," + latitude);
        }
        public Point3D Change(Drugstore point)
        {
            List<double> listCoordinate = ChangeToCoordinate(point);
            return ChangeCoordinateMapToCoordinate3D(listCoordinate[0], listCoordinate[1]);
        }
        public double GetDistance(double longitude1, double latitude1, double longitude2, double latitude2)
        {
            Point3D X = ChangeCoordinateMapToCoordinate3D(longitude1, latitude1);
            Point3D Y = ChangeCoordinateMapToCoordinate3D(longitude2, latitude2);
            return GetDistance(X, Y);
        }
        public double GetDistance(Point3D pointA, Point3D pointB)
        {
            double xx = pointA.x - pointB.x;
            double yy = pointA.y - pointB.y;
            double zz = pointA.z - pointB.z;
            return Math.Sqrt(xx * xx + yy * yy + zz * zz);
        }
        public double GetDistance(Point2D pointA, Point2D pointB)
        {
            double xx = pointA.x - pointB.x;
            double yy = pointA.y - pointB.y;
            return Math.Sqrt(xx * xx + yy * yy);
        }
        public List<Tuple<List<Drugstore>, string>> DoKMeans(List<Drugstore> listStore, int clusterCount)
        {
            var allGroups = new List<List<Drugstore>>();
            var result = new List<Tuple<List<Drugstore>, string>>();
            //devide all group into some group
            for (int i = 0; i < clusterCount; i++)
            {
                var tempList = new List<Drugstore>();
                for (int j = 0; j < listStore.Count; j++)
                {
                    if (j % clusterCount == i) tempList.Add(listStore[j]);
                }
                allGroups.Add(tempList);
            }
            int movement = 1;
            int movementMax = 0;
            //while (movement > 0 && movementMax<100)
            //{
            while (movement > 0 )
            {
                movement = 0;
                movementMax++;
                for (int i = 0; i < allGroups.Count; i++)
                {
                    List<Drugstore> tempList = allGroups[i];
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        Drugstore tempPoint = tempList[j];
                        int neareastCluster = -1;
                        double currDistance = double.MaxValue;
                        for (int k = 0; k < allGroups.Count; k++)
                        {
                            double findDistance = FindDistance2D(allGroups[k], tempPoint);
                            if (findDistance < currDistance)
                            {
                                neareastCluster = k;
                                currDistance = findDistance;
                            }
                        }
                        //if (neareastCluster != j)
                        if (neareastCluster != i)
                        {
                            if (tempList.Count > 1)
                            {
                                tempList.Remove(tempPoint);
                                allGroups[neareastCluster].Add(tempPoint);
                                movement++;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < allGroups.Count; i++)
            {
                result.Add(
                new Tuple<List<Drugstore>, string>
                (
                    allGroups[i],
                    ChangeCoordinate2DToCoordinateMap(FindCentroid2D(allGroups[i]))
                )
                );
            }
            return result;
        }
        //public Drugstore NeareastToCentroid(List<Drugstore> listStore)
        //{
        //    var centroid = FindCentroid(listStore);
        //    double curDistance = double.MaxValue;
        //    var result = new Drugstore();
        //    for (int i = 0; i < listStore.Count; i++)
        //    {
        //        if (GetDistance(centroid, Change(listStore[i])) < curDistance)
        //        {
        //            result = listStore[i];
        //        }
        //    }
        //    return result;
        //}
        public Point3D FindCentroid(List<Drugstore> listStore)
        {
            int number = listStore.Count();
            double X = 0, Y = 0, Z = 0;
            for (int i = 0; i < number; i++)
            {
                var temp = ChangeToCoordinate(listStore[i]);
                var point3D = ChangeCoordinateMapToCoordinate3D(temp[0], temp[1]);
                X += point3D.x;
                Y += point3D.y;
                Z += point3D.z;
            }
            return new Point3D
            {
                x = X / number,
                y = Y / number,
                z = Z / number

            };
        }
        private double FindDistance(List<Drugstore> listStore, Drugstore point)
        {
            var centroid = FindCentroid(listStore);
            var point3D= Change(point);
            return GetDistance(centroid, point3D);
        }
        private double FindDistance2D(List<Drugstore> listStore, Drugstore point)
        {
            var centroid = FindCentroid2D(listStore);
            var temp = ChangeToCoordinate(point);
            var point2D = new Point2D();
            point2D.x = temp[0];
            point2D.y = temp[1];
            return GetDistance(centroid, point2D);
        }
        public Point2D FindCentroid2D(List<Drugstore> listStore)
        {
            int number = listStore.Count();
            double X = 0, Y = 0;
            for (int i = 0; i < number; i++)
            {
                var temp = ChangeToCoordinate(listStore[i]);
                //var point3D = ChangeCoordinateMapToCoordinate3D(temp[0], temp[1]);
                X += temp[0];
                Y += temp[1];
            }
            return new Point2D
            {
                x = X / number,
                y = Y / number,

            };
        }
    }
}