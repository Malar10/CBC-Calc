using System.Numerics;

namespace CBC_Calc
{
	internal class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				CalculateAngles();

                Console.WriteLine("Calculate another shot?");
                Console.WriteLine("y/n");
				string? response = Console.ReadLine();
				if (response.Equals("y", StringComparison.OrdinalIgnoreCase))
				{
                    Console.WriteLine();
				} else 
				{
					break;
				}
			}
		}

		static void CalculateAngles()
		{
			Vector3 cannonpos = getInputPosition("give cannon coordinates:");
			Vector3 targetpos = getInputPosition("\ngive target coordinates:");

			Vector3 localpos = targetpos - cannonpos;
			float yaw = -MathF.Atan2(localpos.X, localpos.Z);
			float yawDeg = yaw * 180 / MathF.PI;
			float dist = MathF.Sqrt(MathF.Pow(localpos.X, 2) + MathF.Pow(localpos.Z, 2));

			Console.WriteLine();
			Console.WriteLine($"Distance: {dist} blocks");
			Console.WriteLine($"Yaw: {yawDeg} degrees ({360+yawDeg} degrees)");
            Console.WriteLine();
			Console.WriteLine("Close pitch angles:");

			var p = new ProjectileEnv();

			for (int charges = 1; charges <= 8; charges++)
			{
				p.charges = charges;

                Console.WriteLine($"Charges: {charges}");
				var closeOnes = p.calc(dist, localpos.Y);

				foreach (Hit hit in closeOnes)
				{
					Console.WriteLine(hit);
				}
                Console.WriteLine();
			}
		}

		static Vector3 getInputPosition(string text)
		{
			while (true)
			{
                Console.WriteLine(text);
                Console.WriteLine("X Y Z");
				string? input = Console.ReadLine();
				if (input == null) 
				{
                    Console.WriteLine("input something buddy");
					continue;
				}

				var n = input.Split(' ', options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				if (n.Length != 3)
				{
                    Console.WriteLine("should have 3 coordinates");
					continue;
				}

				try
				{
					Vector3 pos = new Vector3(float.Parse(n[0]), float.Parse(n[1]), float.Parse(n[2]));

					return pos;
				}
				catch (Exception)
				{
                    Console.WriteLine("not a number");
					continue;
				}
				
			}
		}

		void teststuff()
		{
			Console.WriteLine("Hello, World!");

			float yaw = 90f;
			float pitch = -74.3f;
			float vel = 240f / 20f;
			bool ismortarstone = false;
			Vector3 cannonPos = new Vector3(0, 0, 0);
			float cannonLength = 0f;

			//bear hole from shooting range
			//float yaw = 45.6f;
			//float pitch = -60f;
			//float vel = 80f / 20f;
			//bool ismortarstone = false;
			//Vector3 cannonPos = new Vector3(1228.5f, 74.5f, -506.5f);
			//float cannonLength = 3f;

			Vector3 direction = new Vector3(0, 0, 1);
			Quaternion rotQuat = Quaternion.CreateFromYawPitchRoll(-yaw * MathF.PI / 180, pitch * MathF.PI / 180, 0);
			direction = Vector3.Normalize(Vector3.Transform(direction, rotQuat));
			Console.WriteLine($"direction: {direction}");

			Vector3 shootpos = cannonPos + (direction * cannonLength);
			Console.WriteLine($"shootpos {shootpos}");


			Projectile shell = new Projectile(shootpos, direction * vel, ismortarstone);

			int i = 0;
			while (true)
			{
				i++;
				Console.WriteLine($"\ntick {i}");
				shell.Tick();

				if (shell.position.Y <= cannonPos.Y)
				{
					Console.WriteLine($"hit ground at {shell.position}");
					string temp = $"/tp malar_io {shell.position.X} {shell.position.Y} {shell.position.Z}";
					Console.WriteLine(temp.Replace(",", "."));
					break;
				}
			}

			if (false)
			{
				int a = 7;
				int ao = a;
				int b = 2;
				int c = 6;
				for (int n = 1; n < 100; n++)
				{
					a = a * b + c;

					//geometric series or whatever the fuck
					var sum1 = (c * (1 - Math.Pow(b, n))) / (1 - b);
					var sum2 = sum1 + ao * Math.Pow(b, n);
					Console.WriteLine($"{n}: {a}      {sum2}");
				}
			}
		}

		
	}

	class Projectile
	{
		//params
		float gravity = -0.05f; //gravity multiplier 0 in overworld
		float drag = 0.01f;
		bool isQuadraticDrag = false;
		Vector3 gravityVec = new Vector3(0f, -0.05f, 0f);


		public Vector3 position;
		public Vector3 testposition;
		Vector3 deltaMovement;
		Vector3 startVelocity;
		Vector3 startPosition;

		int t = 0;

		public Projectile(Vector3 position, Vector3 velocity, bool ismortarstone)
		{
			this.position = position;
			this.testposition = position;
			this.startPosition = position;
			this.deltaMovement = velocity;
			this.startVelocity = velocity;
            Console.WriteLine($"startVelocity: {startVelocity}");

			if (ismortarstone)
			{
				gravity = -0.025f;
				gravityVec = new Vector3(0f, -0.025f, 0f);
			}
		}

		//idk
		void setPos(Vector3 newPos)
		{
			this.position = newPos;
            Console.WriteLine($"position: {position}");
		}

		public void Tick()
		{
			t++;

			
			//Vector3 oldVel = this.GetDeltaMovement();
			Vector3 oldPos = this.position;
			//Console.WriteLine($"oldVel         {oldVel}");

			//get velocity
			Vector3 oldVel = GetVelocity(t);
			Vector3 nextVelocity = GetVelocity(t + 1);
			Console.WriteLine($"calculated vel {oldVel}");

			//testposition += nextVelocity;
			//Console.WriteLine($"testposition: {testposition}");
			Console.WriteLine($"posXIntegral: {PosXIntegral(t)}");
			Console.WriteLine($"posX: {GetPosX(t)}");

			Console.WriteLine($"posYIntegral: {PosYIntegral(t)}");
			Console.WriteLine($"posY: {GetPosY(t)}");

			//Vector3 accel = this.GetForces(oldPos, oldVel);
			//Vector3 accel = nextVelocity - oldVel;
			//Console.WriteLine($"acceleration {accel}");
			//Vector3 newPos = oldPos + oldVel + (accel * 0.5f);
			Vector3 newPos = oldPos + 0.5f*(oldVel + nextVelocity);
			this.setPos(newPos);
			//this.setDeltaMovement(oldVel + accel);
		}


		Vector3 GetVelocity(int tick)
		{
			Vector3 sum1 = (gravityVec * (1 - MathF.Pow(1 - drag, tick - 1))) / drag;
			Vector3 sum2 = sum1 + startVelocity * MathF.Pow(1 - drag, tick - 1);

			return sum2;
		}

		float PosXIntegral(int x)
		{
			float a = startVelocity.X;
			float b = drag;

			float integral = -(a * (b - 2) * MathF.Pow(1 - b, x)) / (2 * MathF.Log(1 - b));

			return integral;
		}

		float GetPosX(int x)
		{
			return PosXIntegral(x) - PosXIntegral(0);
		}

		float PosYIntegral(int x)
		{
			float G = gravity;
			float D = drag;
			float v = startVelocity.Y;

			float guh = ((D - 2) * MathF.Pow(1 - D, x - 1) * (D * v - G)) / MathF.Log(1-D);
			return (2 * G * x - guh) / (2 * D);
		}

		float GetPosY(int x)
		{
			return PosYIntegral(x) - PosYIntegral(0);
		}

		Vector3 GetDeltaMovement()
		{
			//movement of actual minecraft projectile?
			return this.deltaMovement;
		}

		void setDeltaMovement(Vector3 deltaMovement)
		{
			this.deltaMovement = deltaMovement;
		}

		protected float GetDragForce()
		{
			float vel = this.GetDeltaMovement().Length();
			float fromDrag = this.drag;
			float density = 1f; 
			//things to add if i wanna
			//density differs per dimension. 1 in overworld
			//fluid drag also here

			float drag = fromDrag * density * vel;
			if (this.isQuadraticDrag)
				drag *= vel;

			return Math.Min(drag, vel); //drag is always smaller, this does nothing rn
		}

		protected Vector3 GetForces(Vector3 position, Vector3 velocity)
		{
			//Vector3 dir = Vector3.Normalize(velocity);
			//Vector3 drag = dir * -this.GetDragForce();
			//Vector3 final = drag + new Vector3(0, this.gravity, 0);

			Vector3 final = (-velocity * this.drag) + new Vector3(0, this.gravity, 0);

			return final;
		}


		Vector3 VectorExp(Vector3 v, float exp)
		{

			return new Vector3(MathF.Pow(v.X, exp), MathF.Pow(v.Y, exp), MathF.Pow(v.Z, exp));
		}
	}

	

	class ProjectileEnv
	{
		float D = 0.99f; //1 - drag
		float G = -0.05f; //gravity
		float v; //start vel

		float maxError = 5f;

		public int charges { set => v = value * 40f/20f; }

		public ProjectileEnv()
		{
			charges = 1;
			//v = 160f / 20f; //start vel
		}

		public List<Hit> calc(float desired_x, float desired_y)
		{

			float t = 0;
			List<Hit> closeOnes = new();

			while (true)
			{
				//this is silly

				t = t + 1;
				//Console.Write($"\n{t}: ");
				if (t > 1000) break;

				float thingy = anglethingy(t, desired_y);
				if (thingy < -1 || thingy > 1) continue; //height not reachable at this angle

				float angle = MathF.Asin(thingy);
				//Console.Write($"angle: {angle}, ");

				float x = getXAtTAndAngle(t, angle);
				//Console.Write($"x: {x}");


				if (MathF.Abs(x - desired_x) < maxError) //close
				{
					closeOnes.Add(new Hit(x, x - desired_x, desired_y, t, angle));
				}
			}

			return closeOnes;
		}

		float anglethingy(float t, float y)
		{
			float upper = -G * MathF.Pow(D, t) + MathF.Log(D) * ((D - 1) * y + G * t) + G;
			float lower = (D - 1) * v * (MathF.Pow(D, t) - 1);

			return upper / lower;
		}

		float getXAtTAndAngle(float t, float a)
		{
			// D != 1
			float x = v * MathF.Cos(a) * (MathF.Pow(D, t) - 1) / MathF.Log(D);
			return x;
		}
	}

	class Hit
	{
		float t;
		float angle;
		float x;
		float xError;
		float y;

		public Hit(float _x, float _xError, float _y, float _t, float _angle)
		{
			x = _x;
			xError = _xError;
			y = _y;
			t = _t;
			angle = _angle;
		}

		public override string ToString()
		{
			float degrees = angle * 180 / MathF.PI;

			string distString = $"error={xError.ToString("n1").PadLeft(4)}m".PadRight(15);
			string pitchString = $"pitch={degrees.ToString("n2").PadLeft(5)}°".PadRight(15);
			string timeString = $"t={t.ToString().PadLeft(5)} ticks";

			return $"Hit: {pitchString} {distString} {timeString}";
		}
	}
}
