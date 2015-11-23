using System;
using System.Windows.Forms;
using System.Drawing;

class TimerDemo:Form
{
  Timer Clock;
  Label lbTime=new Label();

  public TimerDemo()
  {
    Clock=new Timer();
    Clock.Interval=1000;
    Clock.Start();
    Clock.Tick+=new EventHandler(Timer_Tick);

    this.Controls.Add(lbTime);
    lbTime.BackColor=Color.Black;
    lbTime.ForeColor=Color.Red;
    lbTime.Font=new Font("Times New Roman",15);
    lbTime.Text=GetTime();
  }

  public string GetTime()
  {
    string TimeInString="";
    int hour=DateTime.Now.Hour;
    int min=DateTime.Now.Minute;
    int sec=DateTime.Now.Second;

    TimeInString=(hour < 10)?"0" + hour.ToString() :hour.ToString();
    TimeInString+=":" + ((min<10)?"0" + min.ToString() :min.ToString());
    TimeInString+=":" + ((sec<10)?"0" + sec.ToString() :sec.ToString());
    return TimeInString;
  }

  public void Timer_Tick(object sender,EventArgs eArgs)
  {
    if(sender==Clock)
    {
      lbTime.Text=GetTime();      
    }
  }

  public static void Main()
  {
    Application.Run(new TimerDemo());
  }
}
