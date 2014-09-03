namespace UiGuidelineUnitSelection.CommonControl
{
    /// <summary>
    /// ユニット一覧表示用のコントロール。
    /// サンプルだし凝った表示してない。ユニットの画像、ID、レベルを <see cref="System.Windows.Controls.WrapPanel"/> で並べておしまい。
    ///
    /// 実際には、属性でフィルタリングしたり、いろんな条件でソートしたり、状況に応じて選択の可否/表示の有無を変えたりとかの機能が入る。
    /// この手の条件は <see cref="ViewModels.UnionPageModel"/> あたりに持つ想定。
    /// </summary>
    public partial class UnitListControl
    {
        public UnitListControl()
        {
            InitializeComponent();
        }
    }
}
