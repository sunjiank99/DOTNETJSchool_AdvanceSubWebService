using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Security;

namespace WebServeAddvanceSub
{
    /// <summary>
    /// database 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://app8.sngoo.com.cn/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class database : System.Web.Services.WebService
    {
        public class GoodsOrder
        {

            public string OrderNo;
            public DateTime PayTime;
            public string PickPointName;
            public string RecevieName;
            public string ReceiveTel;
            public byte AddvanceSub;  //是否预分装 0默认未进行预分装 1已经进行预分装
            public string PickCode;//下单人推荐码
            public string RecoCode;// 推荐码
            public string ShoppingNo;//发货单号
        }

        public class GoodsOrderInfo
        {
            public string GoodsName;
            public int GoodsNum;

        }

        public class AddvanceSubInfo
        {
            public int id;
            public int OrderNum;
            public DateTime CountTime;
            public string Details;
            public int AddvanceSubState;
            public string AddvanceId;

        }

        //箱子信息类
        public class BoxInfo
        {
            public string BoxId;
            public string OrderNum;
            public string BoxGroupId;  //柜子编号
            public string BoxGroupOpenCode; //箱子编号

            public int SendState;   // 投递装填


           
        }
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }


        [WebMethod]
        /// <summary>
        /// 验证用户名密码
        /// </summary>
        /// <param name="id">用户名</param>
        /// <param name="ps">密码</param>
        /// <returns>true正确 false 错误</returns>
        public bool verifyPassWord(string id, string ps)
        {

            string ConnString = ConfigurationManager.AppSettings["SCYueve"];
            string inputPs, readPS;
            bool returnVal;
            string ConnQuery = "select " + "UserPwd" + " from " + "ES_User" + " where " + "UserLogin" + "='" + id + "'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            reader.Read();
            if (!reader.HasRows) //读取的行数
            {
                returnVal = false;
            }
            else
            {
                readPS = reader[0].ToString();
                inputPs = FormsAuthentication.HashPasswordForStoringInConfigFile(ps, "MD5");
                //string strMd5 = HashPasswordForStoringInConfigFile("123", "md5"); 


                if (readPS == inputPs)
                {
                    returnVal = true;

                }
                else
                {
                    returnVal = false;
                }



            }

            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;



        }


        [WebMethod]
        /// <summary>
        /// 验证是否有登入权限
        /// </summary>
        /// <param name="id">登入账号</param>
        /// <returns></returns>
        public bool verifyUserRole(string id)
        {

            string ConnString = ConfigurationManager.AppSettings["SCYueve"];
            //string inputPs, readPS;
            bool returnVal = false;
            string ConnQuery = "select " + "RoleNames" + " from " + "ES_User" + " where " + "UserLogin" + "='" + id + "'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            reader.Read();
            string RoleRead = reader["RoleNames"].ToString();
            string[] RoleReadArray = RoleRead.Split(',');
            foreach (string i in RoleReadArray)
            {
                if (i == "配送经理" || i == "分装员")
                {
                    returnVal = true;
                    break;
                }
            }



            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;



        }


        [WebMethod]
        /// <summary>
        /// 读取提货点信息
        /// </summary>
        /// <returns></returns>
        public List<string> getPickPointList()
        {

            List<string> returnVal = new List<string>();
            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            //string inputPs, readPS;

            string ConnQuery = "select " + "PointName" + " from " + "yl_DrugPickPoint";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            while (reader.Read())
            {

                returnVal.Add(reader["PointName"].ToString());



            }



            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;



        }



        [WebMethod]
        /// <summary>
        /// 得到与预分装订单信息
        /// </summary>
        /// <param name="PickPoint"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<GoodsOrder> getGoodsOrder(string PickPoint, DateTime starTime, DateTime endTime)
        {
            List<GoodsOrder> returnVal = new List<GoodsOrder>();

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            //string inputPs, readPS;

            string ConnQuery = "  select * from yl_GoodsOrder where (PayTime between '" + starTime + "' and '" + endTime + "') and PickPointName='" + PickPoint + "' and (OrderState=1 and ShoppingState=0 and PayState=2)  ";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            while (reader.Read())
            {
                GoodsOrder item = new GoodsOrder();
                item.OrderNo = reader["OrderNo"].ToString();
                item.PayTime = Convert.ToDateTime(reader["PayTime"]);
                item.PickPointName = reader["PickPointName"].ToString();
                item.ReceiveTel = reader["ReceiveTel"].ToString();
                item.RecevieName = reader["RecevieName"].ToString();
                if (reader["CheckerCode"].ToString() == "1")
                {

                    item.AddvanceSub = 1;
                }
                else if (reader["CheckerCode"].ToString() == "0" || reader["CheckerCode"].ToString()=="")
                {
                    item.AddvanceSub = 0;
                }
                if (item.AddvanceSub == 0)
                {
                    returnVal.Add(item);
                }
                


            }



            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;




        }

        [WebMethod]
        /// <summary>
        /// 按时间搜索所有订单
        /// </summary>
        /// <param name="starTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public List<GoodsOrder> getALLGoodsOrder(DateTime starTime, DateTime endTime)
        {
            List<GoodsOrder> returnVal = new List<GoodsOrder>();

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            //string inputPs, readPS;

            string ConnQuery = "  select * from yl_GoodsOrder where (PayTime between '" + starTime + "' and '" + endTime + "') and (OrderState=1 and ShoppingState=0 and PayState=2)  ";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            while (reader.Read())
            {
                GoodsOrder item = new GoodsOrder();
                item.OrderNo = reader["OrderNo"].ToString();
                item.PayTime = Convert.ToDateTime(reader["PayTime"]);
                item.PickPointName = reader["PickPointName"].ToString();
                item.ReceiveTel = reader["ReceiveTel"].ToString();
                item.RecevieName = reader["RecevieName"].ToString();
                if (reader["CheckerCode"].ToString() == "1")
                {

                    item.AddvanceSub = 1;
                }
                else if (reader["CheckerCode"].ToString() == "0" || reader["CheckerCode"].ToString() == "")
                {
                    item.AddvanceSub = 0;
                }
                if (item.AddvanceSub == 0)
                {
                    returnVal.Add(item);
                }



            }



            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;


        }

        [WebMethod]
        /// <summary>
        /// 得到订单详情
        /// </summary>
        /// <param name="GoodsNo">订单号</param>
        /// <returns></returns>

        public List<GoodsOrderInfo> getGoodsOrderInfo(string GoodsNo)
        {

            List<GoodsOrderInfo> returnVal = new List<GoodsOrderInfo>();

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            //string inputPs, readPS;

            string ConnQuery = "  select * from yl_GoodsOrderInfo where OrderNo='" + GoodsNo + "' ";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            while (reader.Read())
            {
                GoodsOrderInfo item = new GoodsOrderInfo();
                item.GoodsName = reader["GoodsName"].ToString();
                item.GoodsNum = Convert.ToInt32(reader["GoodsNum"]);
                returnVal.Add(item);


            }



            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;





        }


        [WebMethod]
        /// <summary>
        /// 更改订单与分装状态
        /// </summary>
        /// <param name="GoodsNo">订单号</param>
        /// <param name="Status">订单状态</param>
        public void alterAddvanceSubStatus(string GoodsNo, string Status)
        {


            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            string ConnQuery = "  update yl_GoodsOrder set CheckerCode='" + Status + "' where OrderNo='" + GoodsNo + "'";

            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            lo_cmd.ExecuteNonQuery();




            connection.Close();
            connection.Dispose();


        }

        [WebMethod]
        /// <summary>
        /// 写入预分装列表
        /// </summary>
        /// <param name="info">与分装信息</param>
        public void WriteIntoSub(AddvanceSubInfo info)
        {


            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            string ConnQuery = "  insert into  AddvanceSub (CountTime,OrderNum,Details,AddvanceSubState,AddvanceId) values('" + info.CountTime + "'," + info.OrderNum + ",'" + info.Details + "'," + info.AddvanceSubState +",'"+info.AddvanceId+"')";

            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            lo_cmd.ExecuteNonQuery();




            connection.Close();
            connection.Dispose();


        }



        [WebMethod]
        /// <summary>
        /// 读取预分装列表
        /// </summary>
        /// <returns></returns>
        public List<AddvanceSubInfo> ReadSubInfo()
        {

            List<AddvanceSubInfo> returnVal = new List<AddvanceSubInfo>();

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            //string inputPs, readPS;

            string ConnQuery = "  select * from AddvanceSub ";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            while (reader.Read())
            {
                AddvanceSubInfo item = new AddvanceSubInfo();
                item.AddvanceSubState = Convert.ToInt32(reader["AddvanceSubState"]);
                item.CountTime = Convert.ToDateTime(reader["CountTime"]);
                item.Details = reader["Details"].ToString();
                item.id = Convert.ToInt32(reader["id"]);
                item.OrderNum = Convert.ToInt32(reader["OrderNum"]);
                item.AddvanceId = reader["AddvanceId"].ToString();
                returnVal.Add(item);


            }



            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;



        }

        [WebMethod]
        /// <summary>
        /// 更改预分装状态
        /// </summary>
        /// <param name="Status">状态</param>
        /// <param name="id">与分装序号</param>
        public void AlterSubStatus(int Status, int id)
        {

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            string ConnQuery = "  update AddvanceSub set AddvanceSubState='" + Status + "' where id='" + id + "'";

            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            lo_cmd.ExecuteNonQuery();




            connection.Close();
            connection.Dispose();


        }

        [WebMethod]
        /// <summary>
        /// 删除预分装信息
        /// </summary>
        /// <param name="id">预分装序号</param>

        public void DeleteSubInfo(int id)
        {

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            string ConnQuery = " delete  AddvanceSub where id='" + id + "'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            lo_cmd.ExecuteNonQuery();




            connection.Close();
            connection.Dispose();


        }


        [WebMethod]
        /// <summary>
        /// 写入订单关联表
        /// </summary>
        /// <param name="list">统计生成的订单号表</param>
        /// <param name="id">预分装对应id</param>>
        public void InputAssociationList(List<string> list, string id)
        {

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();

            for (int i = 0; i < list.Count; i++)
            {

                string ConnQuery = "  insert into  yl_GoodsOrder_Plus (OrderNo,AddvanceId) values('" + list[i].ToString() + "','" + id + "')";


                SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
                lo_cmd.ExecuteNonQuery();
            }



            connection.Close();
            connection.Dispose();


        }


        [WebMethod]
        /// <summary>
        /// 根据预分拣账号获得对应订单列表
        /// </summary>
        /// <param name="AddvanceId">预分拣账号</param>
        /// <returns></returns>
        public List<string> getOrderList(string AddvanceId)
        {
            List<string> returnVal = new List<string>();
            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            //string inputPs, readPS;

            string ConnQuery = "select " + "OrderNo" + " from " + "yl_GoodsOrder_Plus where AddvanceId='" + AddvanceId + "'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            while (reader.Read())
            {

                returnVal.Add(reader["OrderNo"].ToString());



            }



            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;




        }


        [WebMethod]
        /// <summary>
        /// 批量修改订单预分拣状态
        /// </summary>
        /// <param name="Status">状态 0为未进行 1为已进行预分装</param>
        /// <param name="GoodsOrder">订单号</param>
        public void AlterGoodsOderStatus(int Status, List<string> GoodsOrder)
        {

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];


            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();


            for (int i = 0; i < GoodsOrder.Count; i++)
            {
                string ConnQuery = "  update yl_GoodsOrder set CheckerCode='" + Status + "' where OrderNo='" + GoodsOrder[i].ToString() + "'";
                SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
                lo_cmd.ExecuteNonQuery();
            }




            connection.Close();
            connection.Dispose();



        }



        [WebMethod]
        /// <summary>
        /// 根据预分装号获取的订单列表获取每条订单的详细信息列表
        /// </summary>
        /// <param name="list"> 订单列表</param>
        /// <returns></returns>
        public List<GoodsOrder> getGoodsOrderListByAddvanceId(List<string> list)
        {

            List<GoodsOrder> returnVal = new List<GoodsOrder>();

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            //string inputPs, readPS;


            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            for (int i = 0; i < list.Count; i++)
            {
                string ConnQuery = "  select * from yl_GoodsOrder where OrderNo= '" + list[i].ToString() + "'";
                SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);

                SqlDataReader reader = lo_cmd.ExecuteReader();
                reader.Read();
                GoodsOrder item = new GoodsOrder();
                item.OrderNo = reader["OrderNo"].ToString();
                item.PayTime = Convert.ToDateTime(reader["PayTime"]);
                item.PickPointName = reader["PickPointName"].ToString();
                item.ReceiveTel = reader["ReceiveTel"].ToString();
                item.RecevieName = reader["RecevieName"].ToString();
                if (reader["CheckerCode"].ToString() != "0" || reader["CheckerCode"].ToString() == "")
                {

                    item.AddvanceSub = 1;
                }
                else
                {
                    item.AddvanceSub = 0;
                }
                if (reader["PickCode"].ToString() == "")
                {
                    item.PickCode = "";
                }
                else
                {

                    item.PickCode = reader["PickCode"].ToString();
                }

                if (reader["RecoCode"].ToString() == "")
                {
                    item.RecoCode = "";
                }
                else
                {

                    item.RecoCode = reader["RecoCode"].ToString();
                }
                if (reader["ShoppingNo"].ToString() == "")
                {
                    item.ShoppingNo = "";
                }
                else
                {
                    item.ShoppingNo = reader["ShoppingNo"].ToString();
                }





                returnVal.Add(item);

                reader.Close();
                lo_cmd.Dispose();
            }






            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;







        }


        [WebMethod]
        /// <summary>
        /// 修改订单发货状态
        /// </summary>
        /// <param name="Status">状态吗</param>
        /// <param name="GoodsOrder">订单列表</param>
        public void alterGoodsOrderShoppingState(int Status, List<string> GoodsOrder)
        {


            string ConnString = ConfigurationManager.AppSettings["Sngoo"];


            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();


            for (int i = 0; i < GoodsOrder.Count; i++)
            {
                string ConnQuery = "  update yl_GoodsOrder set ShoppingState='" + Status + "' where OrderNo='" + GoodsOrder[i].ToString() + "'";
                SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
                lo_cmd.ExecuteNonQuery();
            }




            connection.Close();
            connection.Dispose();

        }

        [WebMethod]
        /// <summary>
        /// 写入发货单号
        /// </summary>
        /// <param name="No">发货单号</param>
        /// <param name="GoodsNo">对应订单号订单号</param>
        public void WriteShoppingNo(string No,string GoodsNo)
        {
            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            string ConnQuery = "  update yl_GoodsOrder set ShoppingNo='" + No + "' where OrderNo='" + GoodsNo + "'";

            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            lo_cmd.ExecuteNonQuery();




            connection.Close();
            connection.Dispose();

        }



        [WebMethod]
        /// <summary>
        /// 返回水果分类表
        /// </summary>
        /// <returns></returns>
        public List<string> GetGoodsClass()
        { 
        
        
             List<string> returnVal = new List<string>();

             string ConnString = ConfigurationManager.AppSettings["Sngoo"];  //设置数据库连接字符

             SqlConnection connection = new SqlConnection(ConnString);       //启动连接
             connection.Open();
            //string inputPs, readPS;
              string ConnQuery = "  select 物品分类 from 物品信息_主表 ";  //设置查询语句

              SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);

                SqlDataReader reader = lo_cmd.ExecuteReader();
                reader.Read();
                returnVal.Add(reader[0].ToString());
                


                while (reader.Read())
                {
                    //foreach (string isSame in returnVal)
                    //{
                    //    if (isSame != reader[0].ToString())    //如果不相等就添加一项
                    //    {
                    //        returnVal.Add(reader[0].ToString());
                    //    }
                    
                    
                    
                    //}

                    returnVal.Add(reader[0].ToString());
                      
                
                
                
                }

                reader.Close();
                lo_cmd.Dispose();

                return returnVal;
        
        
        }
        [WebMethod]
        /// <summary>
        ///  返回箱子编号
        /// </summary>
        /// <param name="GoodsNo">发货单号</param>
        /// <returns></returns>
        public BoxInfo GetBoxInfo(string GoodsNo)
        {
            BoxInfo returnVal = new BoxInfo();

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];  //设置数据库连接字符

            SqlConnection connection = new SqlConnection(ConnString);       //启动连接
            connection.Open();
            //string inputPs, readPS;
            string ConnQuery = "  select * from c_boxOrder where OrderNum='" + GoodsNo + "' ";  //设置查询语句

            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);

            SqlDataReader reader = lo_cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();

                returnVal.BoxGroupId = reader["BoxGroupId"].ToString();
                returnVal.BoxGroupOpenCode = reader["BoxGroupOpenCode"].ToString();
                returnVal.BoxId = reader["BoxId"].ToString();
                returnVal.OrderNum = reader["OrderNum"].ToString();
                if (reader["SendState"].ToString() == "")
                {
                    returnVal.SendState = -1;
                }
                else
                {
                    returnVal.SendState = Convert.ToInt32(reader["SendState"]);
                }
            }

            reader.Close();
            lo_cmd.Dispose();

            return returnVal;
        
        }


    }
}
