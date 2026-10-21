using System.Windows.Input;

namespace PCL;

public partial class PageSetupFeedback
{
    public PageSetupFeedback()
    {
        InitializeComponent();
    }

    private void Feedback_Click(object sender, MouseButtonEventArgs e)
    {
        PageSetupLeft.TryFeedback();
    }
}
