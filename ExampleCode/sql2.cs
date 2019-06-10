using System.Data.SqlClient;

class Sql2 {
  private SqlConnection connection;

  public void test1(string concat) {
    string cmd = "SELECT * from users";
    SqlCommand sql = new SqlCommand(cmd + concat, this.connection);
  }

  public void test2(int id) {
    string cmd = "SELECT * from users WHERE id = " + id;
    SqlCommand sql = new SqlCommand(cmd, this.connection);
  }

  public void test3() {
    var num = 3;

    if (num > 0) {
      string cmd = "SELECT * from users WHERE id = ";
      SqlCommand sql = new SqlCommand(cmd + num, this.connection);
    }
  }
  
  public void test4() {
    string cmd = "SELECT * from users WHERE id = ";
    SqlCommand sql = new SqlCommand(cmd + this.getNumber(), this.connection);
  }

  private int getNumber() {
    return 24;
  }
}
