using System;
using System.Data.SqlClient;

class Jon1 {
  var v4 = true;

  public void test1() {
    string str1 = "test1";
    string str2 = "test2";
    string str3 = str1 + str2 + "test3";
  }

  public void test2(string concat) {
    string cmd = "test1" + concat;
  }

  public void test3() {
      int i1 = 1;
      int i2 = 1;
      int i3 = i1 + i2 + i3;
  }

  public void test4(int concat)
  {
      int i = 1 + concat;
  }

  public void test5() {
      var v1 = "string";
      var v2 = 3;
      var v3 = 3.14;
      var test = v1 + v2 + v3 + v4;
  }

  public void test6() {
    SqlConnection connection;
    var command = new SqlClient.SqlCommand("test" + 3.14, connection);
  }
}
