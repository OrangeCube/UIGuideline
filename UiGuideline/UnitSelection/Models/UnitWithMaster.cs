using System.Collections.Generic;
using System.Linq;

namespace UiGuidelineUnitSelection.Models
{
	/// <summary>
	/// <see cref="Unit"/> と <see cref="MasterData"/> をペアで持っておく。
	/// まあ、何かしらマスターデータを参照する必要あるんだけども、もう最初にペアリングしておくのが楽だった。
	/// </summary>
	public class UnitWithMaster
	{
		/// <summary>
		/// パラメーターの成長率。
		/// ここでは定数でやってるけども、実際にはこれもマスターデータから引く。
		/// レアリティによって違ったりする。
		/// </summary>
		const double GrowthRate = 0.03;

		public UnitWithMaster(Unit u, MasterData masters)
		{
			Unit = u;
			Masters = masters;
			Master = masters.Units.FirstOrDefault(x => x.Id == u.MasterId);
		}

		public Unit Unit { get; private set; }
		public UnitMaster Master { get; private set; }
		public MasterData Masters { get; private set; }

		public int Id { get { return Unit.Id; } }
		public int Level { get { return Unit.Level; } }

		public int Hp { get { return GetLeveledUp(Master.Hp, Unit.Level); } }
		public int Mp { get { return GetLeveledUp(Master.Mp, Unit.Level); } }
		public int Strength { get { return GetLeveledUp(Master.Strength, Unit.Level); } }
		public int Magic { get { return GetLeveledUp(Master.Magic, Unit.Level); } }
		public int Defence { get { return GetLeveledUp(Master.Defence, Unit.Level); } }
		public int Agility { get { return GetLeveledUp(Master.Agility, Unit.Level); } }

		/// <summary>
		/// レベルアップによるパラメーターの補正値を計算。
		/// </summary>
		/// <param name="baseValue">レベル1の時の値。</param>
		/// <param name="level">現在レベル。</param>
		/// <returns>補正済みの値。</returns>
		private int GetLeveledUp(int baseValue, int level)
		{
			var rate = 1 + (level - 1) * GrowthRate;
			return (int)(baseValue * rate);
		}
	}

	public static class UnitExtensions
	{
		public static UnitWithMaster With(this Unit u, MasterData m) { return new UnitWithMaster(u, m); }
		public static IEnumerable<UnitWithMaster> With(this IEnumerable<Unit> units, MasterData m) { return units.Select(u => u.With(m)); }
	}
}
