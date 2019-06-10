class Exec1 {
  public void test1() {
    System.Diagnostics.Process.Start("CMD.exe", "echo Hello");
  }

  public void test2(string name) {
    System.Diagnostics.Process.Start("CMD.exe", "echo " + name);
  }

  public void test3() {
    var data = 4;
    System.Diagnostics.Process.Start("CMD.exe", "echo " + data);
  }
}
