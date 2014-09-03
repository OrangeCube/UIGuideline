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
    public class UnionPageModel : BindableBase
    {
        private readonly MasterData _masters;

        public UnitModel Model { get { return _model; } }
        private readonly UnitModel _model;

        public UnionPageModel(MasterData masters, UnitModel model)
        {
            _masters = masters;
            _model = model;
        }

        // 実際には、Mode.Units を直接返すんじゃなくて、選択可否(ベースに選んだやつを素材としては選択できないとか、カンストしてるやつはベースにできないとか)フラグを持った別クラスを挟む。

        /// <summary>
        /// 合成のベースユニット。
        /// </summary>
        public UnitWithMaster BaseUnit { get { return _BaseUnit; } set { SetProperty(ref _BaseUnit, value); } }
        private UnitWithMaster _BaseUnit;

        /// <summary>
        /// 合成の素材になるユニット一覧。
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<UnitWithMaster> MaterialUnits { get { return _MaterialUnits; } }
        private ObservableCollection<UnitWithMaster> _MaterialUnits = new ObservableCollection<UnitWithMaster>();

        /// <summary>
        /// 強化合成。
        /// </summary>
        public void Enhance()
        {
            _model.Enhance(BaseUnit, MaterialUnits);
        }
    }
}
