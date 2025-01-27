using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Silent_Shadow.Managers;
using Silent_Shadow.Managers.EntityManager;

namespace Silent_Shadow.Models
{
    public class PowerBox : Entity
    {
        private bool canSwitch = true;        // Zustand für Aktionen
        private float switchCooldown = 1f;    // Cooldown-Zeit in Sekunden
        private float cooldownTimer = 0f;     // Timer zum Nachhalten des Cooldowns
        public bool IsPlayerNear { get; private set; } = false; // Spieler in der Nähe?

        public PowerBox(Vector2 position)
        {
            Position = position;
            Sprite = Globals.Content.Load<Texture2D>("Sprites/powerbox");
            Size = 0.03f;
            LayerDepth = 0.07f;
        }

        public override void Update()
        {
            IsPlayerNear = false; // zurücksetzen

            // Reduziere Cooldown-Timer, falls nötig
            if (!canSwitch)
            {
                cooldownTimer += 1 / 60f; // 1/60 für jedes Frame (angenommen 60 FPS)

                if (cooldownTimer >= switchCooldown)
                {
                    canSwitch = true;
                    cooldownTimer = 0f;
                }
            }

            // Spieler in der Nähe überprüfen und Taste "E" abfragen
            float distance = Vector2.Distance(Hero.Instance.Position, Position);
            if (distance < 50f && canSwitch) // Spieler muss nah genug sein
            {
                IsPlayerNear = true; // Spieler ist nah
                if (InputManager.IsKeyPressed(Keys.E))
                {
					IEntityManager entityMgr = EntityManagerFactory.GetInstance();
					entityMgr.Blackout();
                    canSwitch = false;  // Aktion sperren
                    SoundManager.PlaySound("powerbox");
                }
            }

            //Nightvision
            if (InputManager.IsKeyPressed(Keys.F))
            {
                Game1.Instance._NightVision = !Game1.Instance._NightVision; // Boolean umschalten
                canSwitch = false;  // Aktion sperren
            }
        }
    }
}
