using System.Collections.Generic;
using UiGuidelineUnitSelection.Common;
using UiGuidelineUnitSelection.GameModels;
using UiGuidelineUnitSelection.Models;

namespace UiGuidelineUnitSelection.ViewModels
{
    class GachaPageModel : BindableBase
    {
        private readonly MasterData _masters;

        public UnitModel Model { get { return _model; } }
        private readonly UnitModel _model;

        public GachaPageModel(MasterData masters, UnitModel model)
        {
            _masters = masters;
            _model = model;
        }

        /// <summary>
        /// 新規獲得したユニット一覧。
        /// </summary>
        /// <remarks>
        /// 実際には、<see cref="UnitWithMaster"/> で返すんじゃなくて、新規取得化どうかの isNew フラグみたいなのも持った ViewModel を返す。
        /// </remarks>
        public IEnumerable<UnitWithMaster> NewUnits { get { return _NewUnits; } set { SetProperty(ref _NewUnits, value); } }
        private IEnumerable<UnitWithMaster> _NewUnits;

        /// <summary>
        /// ガチャを引く。
        /// </summary>
        public void Draw(bool premiumMode)
        {
            NewUnits = _model.DrawGacha(premiumMode);
        }
    }
}
