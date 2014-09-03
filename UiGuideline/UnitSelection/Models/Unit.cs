namespace UiGuidelineUnitSelection.Models
{
	/// <summary>
	/// ユニットのインスタンスデータ。
	/// </summary>
	public class Unit
	{
		/// <summary>
		/// インスタンスID。
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// マスターID。
		/// </summary>
		public int MasterId { get; set; }

		/// <summary>
		/// 現在レベル。
		/// 実際のゲームだと、通算経験値だけ持っておいて、レベルアップマスターみたいなところから必要経験値を引いて計算して出す。
		/// サンプルだしレベル直持ち。
		/// </summary>
		public int Level { get; set; }
	}
}
