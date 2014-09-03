namespace UiGuidelineUnitSelection.Models
{
	/// <summary>
	/// マスターデータ。
	/// サンプル実装なのでユニットマスターのみ。
	/// 実際にはこれに、アクションとかステータスとかスキル学習・必殺技レベルアップとかのマスターもあるイメージ。
	/// </summary>
	public class MasterData
	{
		public UnitMaster[] Units { get; set; }
	}
}
