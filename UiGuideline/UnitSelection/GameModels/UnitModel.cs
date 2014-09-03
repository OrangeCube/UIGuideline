using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UiGuidelineUnitSelection.Models;

namespace UiGuidelineUnitSelection.GameModels
{
    /// <summary>
    /// クライアント中で使う、ユニット関連のデータ。
    /// </summary>
    /// <remarks>
    /// ちなみに、名前空間の棲み分けとして、
    /// ・Models … 一般論的にいうとエンティティモデルとかになると思う。
    ///            いわゆるデータアクセス層で使う、プレーンな型。
    ///            クライアントの場合はサーバーAPI経由でもらう型なので、通信層の型。
    /// ・GameModels … 一般論的に言うとビジネスロジック層。業務系のいうビジネスロジックは、ゲームだとゲームロジックとか呼ぶ方がよさそう。
    ///                partial が使える(通信層の型を自動生成したとしても、そいつに別途ロジックを乗せれる)C#だと、結構Models側にロジック書けるんだけども。
    ///                基本的には、ゲームクライアント上でのデータ処理はここに書く。
    /// </remarks>
    public class UnitModel
    {
        /// <summary>
        /// 持っているユニット一覧。
        /// </summary>
        public IEnumerable<UnitWithMaster> Units { get { return _units; } }
        private ObservableCollection<UnitWithMaster> _units;

        private readonly MasterData _masters;

        public UnitModel(MasterData masters)
        {
            _masters = masters;

            var units = DummyData.DummyGenerator.GetUnits(masters).With(masters);
            _units = new ObservableCollection<UnitWithMaster>(units);
        }

        /// <summary>
        /// 強化合成する。
        /// </summary>
        /// <param name="baseUnit">強化ベース。</param>
        /// <param name="materialUnits">強化素材一覧。</param>
        public void Enhance(UnitWithMaster baseUnit, IEnumerable<UnitWithMaster> materialUnits)
        {
            // 実際にはサーバーと通信して結果をもらう。

            // このプログラムは所詮はページ遷移関連のコンセプト確認用のダミーなので適当な強化処理する。
            // ・単純に素材になった奴を Units から消す。
            // ・素材のレベルの1割(あまり切り上げ)、ベースのレベルを上げる。

            foreach (var x in materialUnits)
            {
                _units.Remove(x);
                baseUnit.Unit.Level += (int)Math.Ceiling(x.Level * 0.1);
            }
        }

        /// <summary>
        /// ユニットを売る。
        /// </summary>
        /// <param name="units">売りたいユニット一覧。</param>
        public void Sell(IEnumerable<UnitWithMaster> units)
        {
            // お金とか実装してないので、単純にユニット削除。

            foreach (var x in units)
            {
                _units.Remove(x);
            }
        }

        Random _random = new Random();
        public void DrawGacha()
        {
            _units.Add(NewUnit());
        }

        private UnitWithMaster NewUnit()
        {
            var id = Units.Select(u => u.Id).Max() + 1;
            var unit = DummyData.DummyGenerator.GetUnit(_random, _masters, id).With(_masters);
            return unit;
        }


        /// <summary>
        /// ガチャを引く。
        /// </summary>
        public IEnumerable<UnitWithMaster> DrawGacha(bool premiumMode)
        {
            var units = DrawGachaInternal(premiumMode).ToArray();
            foreach (var u in units)
                _units.Add(u);
            return units;
        }

        private IEnumerable<UnitWithMaster> DrawGachaInternal(bool premiumMode)
        {
            // 課金ガチャだと5体、無料ガチャだと1体ユニットを足すだけ。
            // ポイントなんてもの(ダミーには)なかった。

            var n = premiumMode ? 5 : 1;

            for (int i = 0; i < n; i++)
            {
                yield return NewUnit();
            }
        }
    }
}
