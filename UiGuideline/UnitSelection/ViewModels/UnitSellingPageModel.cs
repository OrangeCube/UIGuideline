
using System.Collections.ObjectModel;
using UiGuidelineUnitSelection.Common;
using UiGuidelineUnitSelection.GameModels;
using UiGuidelineUnitSelection.Models;

namespace UiGuidelineUnitSelection.ViewModels
{
    /// <summary>
    /// 合成画面系で必要になるデータ。
    /// </summary>
    /// <remarks>
    /// PageModel って名前になってるやつは、ページ群に紐づいてる(ページ群に入るときに作られて、出てくときに不要になる)ViewModel。
    /// </remarks>
    public class UnitSellingPageModel : BindableBase
    {
        private readonly MasterData _masters;

        public UnitModel Model { get { return _model; } }
        private readonly UnitModel _model;

        public UnitSellingPageModel(MasterData masters, UnitModel model)
        {
            _masters = masters;
            _model = model;
        }

        /// <summary>
        /// 合成の素材になるユニット一覧。
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<UnitWithMaster> Units { get { return _Units; } }
        private ObservableCollection<UnitWithMaster> _Units = new ObservableCollection<UnitWithMaster>();

        /// <summary>
        /// 強化合成。
        /// </summary>
        public void Sell()
        {
            _model.Sell(Units);
        }
    }
}
