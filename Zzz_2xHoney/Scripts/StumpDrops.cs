using HarmonyLib;
using UnityEngine;

namespace zzz_honey
{
	[HarmonyPatch]
	public static class StumpDrops
	{
		// Patch the main block-destroy hook where drops are spawned.
		// Game versions vary; try OnBlockDestroyed in Block and fallback to Block.OnBlockDamaged/PlaceFX loot.
		[HarmonyPatch(typeof(Block), nameof(Block.OnBlockDestroyed))]
		[HarmonyPostfix]
		public static void Block_OnBlockDestroyed_Postfix(Block __instance, 
			WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entity)
		{
			TryForceHoney(__instance, _world, _blockPos, _blockValue);
		}

		// Fallback 1: Some versions route through BlockValue.OnBlockDestroyed (static). Patch that too if present.
		[HarmonyPatch(typeof(BlockValue), nameof(BlockValue.OnBlockDestroyed))]
		[HarmonyPostfix]
		public static void BlockValue_OnBlockDestroyed_Postfix(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
		{
			TryForceHoney(_blockValue.Block, _world, _blockPos, _blockValue);
		}

		// Fallback 2: After block damage leads to destroy, some codepaths call OnBlockDamaged with destroy flags.
		[HarmonyPatch(typeof(Block), nameof(Block.OnBlockDamaged))]
		[HarmonyPostfix]
		public static void Block_OnBlockDamaged_Postfix(Block __instance, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entity, int _damage, bool _bDidDamage)
		{
			if (_bDidDamage && _blockValue.type == 0) // destroyed
			{
				TryForceHoney(__instance, _world, _blockPos, _blockValue);
			}
		}

		private static void TryForceHoney(Block block, WorldBase world, Vector3i pos, BlockValue value)
		{
			var bc = value.Block; // runtime block
			if (bc == null) return;
			var name = bc.GetBlockName();
			if (!name.Equals("treeStump") && !name.Equals("treeStumpPOI")) return;

			// Ensure 2–3 honey at the block position. Use item value from buffs/items db.
			var itemClass = ItemClass.GetItem("foodHoney");
			if (itemClass == null || itemClass.type == ItemValue.None) return;

			// Spawn 2 or 3 (uniform) stacks of 1 to guarantee 2–3 total without stack-size issues.
			int count = Random.Range(0, 100) < 50 ? 2 : 3;
			for (int i = 0; i < count; i++)
			{
				var iv = new ItemValue(itemClass.type, 1, 0, false);
				var dropAt = pos.ToVector3() + new Vector3(0.5f, 0.8f, 0.5f);
				world.GameManager.ItemDropServer(iv, false, dropAt, Vector3.zero, -1, 60f, true);
			}
		}
	}
}


