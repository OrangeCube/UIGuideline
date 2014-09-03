namespace UiGuidelineUnitSelection.Models
{
	public class UnitMaster
	{
		/// <summary>
		/// マスターID。
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 画像ID。
		/// 今回の場合、00000.png みたいな5桁0詰なファイル名の PNG を読む想定。
		/// </summary>
		public int ImageId { get; set; }

		/// <summary>
		/// ユニット名。
		/// </summary>
		public string Name { get; set; }

		#region 初期レベルでのステータス

		// 「1レベルにつき3%ずつ上昇」みたいな想定。

		public int Hp { get; set; }
		public int Mp { get; set; }
		public int Strength { get; set; }
		public int Magic { get; set; }
		public int Defence { get; set; }
		public int Agility { get; set; }

		#endregion
	}
}
