using System.Data.SqlClient;  

class Sql1 {
  private SqlConnection connection;

  public void test1() {
    string cmd = "SELECT * from users";
    SqlCommand sql = new SqlCommand(cmd, this.connection);
  }

  public void test2(string concat) {
    string cmd = "SELECT * from users";
    SqlCommand sql = new SqlCommand(cmd + concat, this.connection);
  }
}
