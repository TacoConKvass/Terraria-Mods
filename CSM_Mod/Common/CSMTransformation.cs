using Xna = Microsoft.Xna.Framework;
using DataStructures = Terraria.DataStructures;
using TML = Terraria.ModLoader;
using Asset = ReLogic.Content.Asset<Microsoft.Xna.Framework.Graphics.Texture2D>;
using UI = Terraria.UI;
using Collections = System.Collections.Generic;

namespace CSM_Mod.Common;

public class Keybinds : TML.ModSystem
{
	public static TML.ModKeybind Transformation;

	public override void Load() {
		Transformation = TML.KeybindLoader.RegisterKeybind(Mod, "CSM_Transformation", Xna.Input.Keys.K);
	}

	public override void Unload() {
		Transformation = null;
	}
}

public class CSM_Transformation : TML.ModPlayer {
	public bool Available;
	public int BloodMeter;

	public int maxBloodMeter = 1200;
	public int bloodPerHit = 20;
	public int damageBoostPercent = 10;
	public int attackSpeedPercent = 10;
	public int hpBoost = 100;
	public int lifeRegenBoost = 20; // HP per 2 seconds
	public int movementSpeedBonusPercent = 15;
	public int transformationDamagePercent = 30;

	const bool BloodBarGoesDownOverTime = true;

	public override void ProcessTriggers(Terraria.GameInput.TriggersSet triggersSet) {
		if (Keybinds.Transformation.JustPressed && Available && BloodMeter <= 0) {
			Player.Hurt(new DataStructures.PlayerDeathReason(), (int)(Player.statLife * (transformationDamagePercent / 100f)), 1, quiet: true, dodgeable: false, knockback: 0, cooldownCounter: 0);
			BloodMeter = maxBloodMeter / 2;
			for (int i = 0; i < 20; i++) {
				Terraria.Dust.NewDust(Player.TopLeft, Player.width, Player.height, Terraria.ID.DustID.Blood);
				Terraria.Dust.NewDust(Player.TopLeft, Player.width, Player.height, Terraria.ID.DustID.Water_Crimson);
			}
		}
	}

	public override void OnHurt(Terraria.Player.HurtInfo info) => BloodMeter -= (info.Damage / 50) * 15;

	public override void OnHitNPC(Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		if (BloodMeter <= 0) return;
		BloodMeter = Terraria.Utils.Clamp(BloodMeter + bloodPerHit, 0, maxBloodMeter);
	}

	public override void UpdateEquips() {
		if (BloodMeter <= 0) return;

		Player.GetAttackSpeed(TML.DamageClass.Generic) += attackSpeedPercent / 100f;
		Player.GetDamage(TML.DamageClass.Generic) += damageBoostPercent / 100f;
		Player.statLifeMax2 += hpBoost;
		Player.accRunSpeed += movementSpeedBonusPercent / 100f;
	}

	public override void UpdateLifeRegen() => Player.lifeRegen += lifeRegenBoost;

	public override void ResetEffects() {
		Available = false;
		int meterLossPerTick = 1;

		if (BloodBarGoesDownOverTime) BloodMeter -= meterLossPerTick;
	}
}

public class BloodBar : UI.UIState
{
	[TML.Autoload(Side = TML.ModSide.Client)]
	public class Setup : TML.ModSystem {
		UI.UserInterface ui;
		BloodBar bar;

		public override void Load() {
			bar = new BloodBar();
			ui = new UI.UserInterface();
			ui.SetState(bar);
		}

		public override void UpdateUI(Xna.GameTime gameTime) {
			ui?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(Collections.List<UI.GameInterfaceLayer> layers) {
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));

			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new UI.LegacyGameInterfaceLayer(
					"CSM_Mod: Blood Bar",
					() => {
						ui.Draw(Terraria.Main.spriteBatch, new Xna.GameTime());
						return true;
					},
					UI.InterfaceScaleType.UI
				));
			}
		}
	}
	
	UI.UIElement area;
	Terraria.GameContent.UI.Elements.UIImage barFrame;
	Xna.Color color;
	Asset bloodBarTexture;

	public override void OnInitialize() {
		bloodBarTexture = TML.ModContent.Request<Xna.Graphics.Texture2D>("CSM_Mod/Assets/BloodBarFrame", ReLogic.Content.AssetRequestMode.ImmediateLoad);
		
		area = new UI.UIElement();
		area.Width.Set(bloodBarTexture.Value.Width, 0f);
		area.Height.Set(bloodBarTexture.Value.Height, 0f);
		area.Left.Set(-bloodBarTexture.Value.Width / 2, 0.5f);
		area.Top.Set(Terraria.Main.LocalPlayer.Hitbox.Height, 0.5f);
		
		// Blood bar frame borrowed from Example Mod, please replace
		// Width and height will need to be adjusted if the texture is replaced
		barFrame = new Terraria.GameContent.UI.Elements.UIImage(bloodBarTexture);
		barFrame.Height.Set(bloodBarTexture.Value.Height, 0f);
		barFrame.Width.Set(bloodBarTexture.Value.Width, 0f);

		color = Xna.Color.Red;

		area.Append(barFrame);
		Append(area);
	}

	public override void Draw(Xna.Graphics.SpriteBatch spriteBatch) {
		// Only draw ig in Chainsawman transformation
		if (Terraria.Main.LocalPlayer.GetModPlayer<CSM_Transformation>().BloodMeter <= 0)
			return;
		
		base.Draw(spriteBatch);
	}

	protected override void DrawSelf(Xna.Graphics.SpriteBatch spriteBatch) {
		base.DrawSelf(spriteBatch);

		CSM_Transformation player = Terraria.Main.LocalPlayer.GetModPlayer<CSM_Transformation>();
		
		float quotient = (float)player.BloodMeter / player.maxBloodMeter;
		quotient = Terraria.Utils.Clamp(quotient, 0f, 1f); 

		// If the texture is changed, these WILL need to be adjusted
		Xna.Rectangle hitbox = barFrame.GetDimensions().ToRectangle();
		hitbox.X += 12;
		hitbox.Width -= 24;
		hitbox.Y += 8;
		hitbox.Height -= 16;

		int left = hitbox.Left;
		int right = hitbox.Right;
		int steps = (int)((right - left) * quotient);
		
		for (int i = 0; i < steps; i++) {
			spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, new Xna.Rectangle{X = left + i, Y = hitbox.Y, Width = 1, Height = hitbox.Height}, color);
		}
	}
}

public class CSM_Layers {
	public class Body : TML.PlayerDrawLayer
	{
		public static Asset BodyTexture { get; set; }
	
		public override void Load() => BodyTexture = TML.ModContent.Request<Xna.Graphics.Texture2D>("CSM_Mod/Assets/Transformation_Body");
	
		public override void Unload() => BodyTexture = null;
	
		public override Position GetDefaultPosition() => new AfterParent(DataStructures.PlayerDrawLayers.Torso);

		public override bool GetDefaultVisibility(DataStructures.PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<CSM_Transformation>().BloodMeter > 0;
	
		protected override void Draw(ref DataStructures.PlayerDrawSet drawInfo) {
			Terraria.Player player = drawInfo.drawPlayer;
			Xna.Color color = player.immune ? Xna.Color.White with { A = (byte)player.immuneAlpha } : Xna.Color.White;

			// Taken from https://github.com/Mr-Plauge/MrPlagueRaces-1.4/
			// Let's assume it's correct :D
			Xna.Vector2 bodyPosition = new Xna.Vector2((int)(drawInfo.Position.X - Terraria.Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2)), (int)(drawInfo.Position.Y - Terraria.Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Xna.Vector2((int)(player.bodyFrame.Width / 2), (int)(player.bodyFrame.Height / 2));
			
			if (!player.invis) {
				DataStructures.DrawData bodyData = new DataStructures.DrawData(BodyTexture.Value, bodyPosition, drawInfo.compTorsoFrame, color, player.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
				drawInfo.DrawDataCache.Add(bodyData);
			}
		}
	}
	
	public class Legs : TML.PlayerDrawLayer {
		public static Asset LegTexture { get; set; }

		public override void Load() => LegTexture = TML.ModContent.Request<Xna.Graphics.Texture2D>("CSM_Mod/Assets/Transformation_Legs");

		public override void Unload() => LegTexture = null;

		public override Position GetDefaultPosition() => new AfterParent(DataStructures.PlayerDrawLayers.Leggings);

		public override bool GetDefaultVisibility(DataStructures.PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<CSM_Transformation>().BloodMeter > 0;

		protected override void Draw(ref DataStructures.PlayerDrawSet drawInfo) {
			Terraria.Player player = drawInfo.drawPlayer;
			Xna.Vector2 helmetOffset = drawInfo.helmetOffset;
			Xna.Color color = player.immune ? Xna.Color.White with { A = (byte)player.immuneAlpha } : Xna.Color.White;

			// Taken from https://github.com/Mr-Plauge/MrPlagueRaces-1.4/
			// Let's assume it's correct :D
			Xna.Vector2 legPosition = new Xna.Vector2((int)(drawInfo.Position.X - Terraria.Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2)), (int)(drawInfo.Position.Y - Terraria.Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Xna.Vector2((int)(player.bodyFrame.Width / 2), (int)(player.bodyFrame.Height / 2));

			if (!player.invis) {

				Xna.Rectangle legFrame = player.legFrame;
				Xna.Vector2 offset = Xna.Vector2.Zero;
				Xna.Vector2 legOffset = Xna.Vector2.Zero;
				if (player.sitting.isSitting) {
					offset.Y += drawInfo.seatYOffset;
					legOffset.Y += 42;
					legOffset.X -= 2;
					DataStructures.DrawData thighData = new DataStructures.DrawData(LegTexture.Value, legPosition + offset + legOffset, legFrame with { Height = 4, Y = 46 }, color, player.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect) ;
					drawInfo.DrawDataCache.Add(thighData);
					legFrame = new Xna.Rectangle { X = 0, Y = 46, Width = legFrame.Width, Height = 8 };
					legOffset.X -= 4;
				}

				DataStructures.DrawData legData = new DataStructures.DrawData(LegTexture.Value, legPosition + offset + legOffset, legFrame, color, player.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
				drawInfo.DrawDataCache.Add(legData);
			}
		}
	}
	
	public class FrontArm : TML.PlayerDrawLayer {
		public override Position GetDefaultPosition() => new AfterParent(DataStructures.PlayerDrawLayers.ArmOverItem);

		public override bool GetDefaultVisibility(DataStructures.PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<CSM_Transformation>().BloodMeter > 0;

		protected override void Draw(ref DataStructures.PlayerDrawSet drawInfo) {
			Asset ArmTexture = Body.BodyTexture;
			Terraria.Player player = drawInfo.drawPlayer;
			Xna.Color color = player.immune ? Xna.Color.White with { A = (byte)player.immuneAlpha } : Xna.Color.White;

			// Taken from https://github.com/Mr-Plauge/MrPlagueRaces-1.4/
			// Let's assume it's correct :D
			Xna.Vector2 frontArmPosition = new Xna.Vector2((int)(drawInfo.Position.X - Terraria.Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2)), (int)(drawInfo.Position.Y - Terraria.Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Xna.Vector2((int)(player.bodyFrame.Width / 2), (int)(player.bodyFrame.Height / 2));
		
			if (!player.invis) {
				DataStructures.DrawData frontArm = new DataStructures.DrawData(ArmTexture.Value, frontArmPosition, drawInfo.compFrontArmFrame, color, drawInfo.compositeFrontArmRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
				drawInfo.DrawDataCache.Add(frontArm);
			}
		}
	}
	
	public class BackArm : TML.PlayerDrawLayer {
		public override Position GetDefaultPosition() => new AfterParent(DataStructures.PlayerDrawLayers.Skin);

		public override bool GetDefaultVisibility(DataStructures.PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<CSM_Transformation>().BloodMeter > 0;

		protected override void Draw(ref DataStructures.PlayerDrawSet drawInfo) {
			Asset ArmTexture = Body.BodyTexture;
			Terraria.Player player = drawInfo.drawPlayer;
			Xna.Color color = player.immune ? Xna.Color.White with { A = (byte)player.immuneAlpha } : Xna.Color.White;

			// Taken from https://github.com/Mr-Plauge/MrPlagueRaces-1.4/
			// Let's assume it's correct :D
			Xna.Vector2 backArmPosition = new Xna.Vector2((int)(drawInfo.Position.X - Terraria.Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2)), (int)(drawInfo.Position.Y - Terraria.Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Xna.Vector2((int)(player.bodyFrame.Width / 2), (int)(player.bodyFrame.Height / 2));
	
			if (!player.invis) {
				DataStructures.DrawData backArm = new DataStructures.DrawData(ArmTexture.Value, backArmPosition, drawInfo.compBackArmFrame, color, drawInfo.compositeBackArmRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
				drawInfo.DrawDataCache.Add(backArm);
			}
		}
	}
	
	public class Head : TML.PlayerDrawLayer {
		public static Asset HeadTexture { get; set; }

		public override void Load() => HeadTexture = TML.ModContent.Request<Xna.Graphics.Texture2D>("CSM_Mod/Assets/Transformation_Head");

		public override void Unload() => HeadTexture = null;

		public override Position GetDefaultPosition() => new AfterParent(DataStructures.PlayerDrawLayers.Head);

		public override bool GetDefaultVisibility(DataStructures.PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<CSM_Transformation>().BloodMeter > 0;

		protected override void Draw(ref DataStructures.PlayerDrawSet drawInfo) {
			Terraria.Player player = drawInfo.drawPlayer;
			Xna.Color color = player.immune ? Xna.Color.White with { A = (byte)player.immuneAlpha } : Xna.Color.White;

			// Taken from https://github.com/Mr-Plauge/MrPlagueRaces-1.4/ and tweaked a bit
			// Let's assume it's correct :D
			Xna.Vector2 headPosition = new Xna.Vector2(
				(int)(drawInfo.Position.X - Terraria.Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2)), 
				(int)(drawInfo.Position.Y - Terraria.Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)
			) + player.headPosition + new Xna.Vector2(14 + (player.direction == 1 ? 5 : 0), 28);
			
			int frameHeight = HeadTexture.Value.Height / 20;
			int headFrame = (player.legFrame.Y / player.legFrame.Height);
			Xna.Rectangle frame = new Xna.Rectangle { X = 0, Y = frameHeight * headFrame	, Height = frameHeight, Width = HeadTexture.Value.Width };

			if (!player.invis) {
				DataStructures.DrawData backArm = new DataStructures.DrawData(HeadTexture.Value, headPosition, frame, color, drawInfo.compositeBackArmRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
				drawInfo.DrawDataCache.Add(backArm);
			}
		}
	}

	public class HideCharacter : TML.PlayerDrawLayer {
		public override Position GetDefaultPosition() => new BeforeParent(DataStructures.PlayerDrawLayers.FirstVanillaLayer);

		public override bool GetDefaultVisibility(DataStructures.PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<CSM_Transformation>().BloodMeter > 0;

		protected override void Draw(ref DataStructures.PlayerDrawSet drawInfo) {
			Xna.Color transparent = Xna.Color.Black with { A = 0 };
			drawInfo.colorArmorBody = transparent;
			drawInfo.colorArmorHead = transparent;
			drawInfo.colorArmorLegs = transparent;
			drawInfo.colorLegs = transparent;
			drawInfo.colorHead = transparent;
			drawInfo.colorEyes = transparent;
			drawInfo.colorEyeWhites = transparent;
			drawInfo.colorHair = transparent;
		}
	}	
}