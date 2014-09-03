using System;
using System.Collections.Generic;
using System.Linq;
using UiGuidelineUnitSelection.Models;

namespace UiGuidelineUnitSelection.DummyData
{
    /// <summary>
    /// 乱数使って適当なデータ作る。
    /// </summary>
    static class DummyGenerator
    {
        public static IEnumerable<Unit> GetUnits(MasterData masters)
        {
            var r = new Random();

            for (int i = 0; i < 100; i++)
            {
                yield return GetUnit(r, masters, i);
            }
        }

        public static Unit GetUnit(Random r, MasterData masters, int id)
        {
            var index = r.Next(masters.Units.Length);
            var u = masters.Units[index];

            var unit = new Unit
            {
                Id = id,
                MasterId = u.Id,
                Level = r.Next(10, 90),
            };
            return unit;
        }

        public static MasterData GetMasters(int count)
        {
            return new MasterData
            {
                Units = GetUnitMasters(count).ToArray()
            };
        }

        private static IEnumerable<UnitMaster> GetUnitMasters(int count)
        {
            var r = new Random();

            for (int i = 0; i < count; i++)
            {
                yield return new UnitMaster
                {
                    Id = i,
                    ImageId = i,
                    Name = i + "番ユニット",
                    Hp = r.Next(500, 1000),
                    Mp = r.Next(200, 400),
                    Strength = r.Next(50, 100),
                    Magic = r.Next(50, 100),
                    Defence = r.Next(50, 100),
                    Agility = r.Next(50, 100),
                };
            }
        }
    }
}
