/// <summary>
/// 2024-07-09 Game Framework Code By Hurley
/// </summary>

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Game
{
    /// <summary>
    /// 测试对象类型
    /// </summary>
    public class TestClassType : ITestCase
    {
        private abstract class Animal
        {
            public static string AnimalTag = "Animal";
            public int uid;
            protected string name;
            private int age = 0;

            public int Uid => uid;
            public string Name => name;
            public int Age { get { return age; } protected set { age = value; } }

            public abstract void SayHello(Animal target);
            public abstract void SayGoodbye(Animal target);
        }

        private class Dog : Animal
        {
            public static string DogName = "people's friend";
            public int speed;
            protected string luckName;
            private int sex;
            protected int health;

            public int Speed => speed;
            public string LuckName => luckName;
            public int Sex { get { return sex; } protected set { sex = value; } }
            public int Health { get { return health; } set { health = value; } }

            public override void SayHello(Animal target)
            {
                Debugger.Warn("汪汪!");
            }

            public override void SayGoodbye(Animal target)
            {
                Debugger.Warn("汪~~");
            }
        }

        private class Hashiqi : Dog
        {
            public static string DogTag = "no anything";
            public int attack;
            private int def;

            public int Attack => attack;
            public int Def => def;

            public Hashiqi()
            {
                this.uid = 1001;
                this.name = "哈士奇";
                this.Age = 2;

                this.speed = 5;
                this.luckName = "拆家小能手";
                this.Sex = 1;
                this.health = 100;

                this.attack = 10;
                this.def = 2;
            }

            public override void SayHello(Animal target)
            {
                //Debugger.Warn("呜呜!");

                health += 1;
                ((Dog) target).Health += 2;
            }

            public override void SayGoodbye(Animal target)
            {
                //Debugger.Warn("呜~~");
                health += 1;
                ((Dog) target).Health += 2;
            }
        }

        private class Alasijia : Dog
        {
            public static string DogTag = "no anything";
            public int attack;
            private int def;

            public int Attack => attack;
            public int Def => def;

            public Alasijia()
            {
                this.uid = 1001;
                this.name = "阿拉斯加";
                this.Age = 2;

                this.speed = 5;
                this.luckName = "雪橇小王子";
                this.Sex = 1;
                this.health = 150;

                this.attack = 8;
                this.def = 4;
            }

            public override void SayHello(Animal target)
            {
                Debugger.Warn("嗷嗷!");
            }

            public override void SayGoodbye(Animal target)
            {
                Debugger.Warn("嗷~~");
            }
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public void Update()
        {
            if (UnityEngine.Input.anyKey)
            {
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A))
                {
                    Test2();
                }
                else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.B))
                {
                    // Test4();
                }
            }
        }

        private void Test1()
        {
            Type targetType = typeof(Hashiqi);

            //while (null != targetType)
            //{
                Debugger.Warn($"------------------------------------ class type = {targetType.FullName} ------------------------------------");
                FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                for (int n = 0; n < fields.Length; ++n)
                {
                    FieldInfo field = fields[n];
                    Debugger.Warn($"field name = {field.Name}, type = {field.FieldType.FullName}");
                }

            //    targetType = targetType.BaseType;
            //}
        }

        private void Test2()
        {
            Debugger.Info("_________________________________________________");
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            Type targetType = typeof(Hashiqi);

            Hashiqi obj = new Hashiqi();
            Alasijia obj2 = new Alasijia();

            stopwatch.Start();
            MethodInfo m1 = targetType.GetMethod("SayHello", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo m2 = targetType.GetMethod("SayGoodbye", BindingFlags.Public | BindingFlags.Instance);

            Delegate d1 = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(obj, m1);
            Delegate d2 = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(obj, m2);
            Delegate d3 = Delegate.Combine(d1, d2);
            stopwatch.Stop();
            Debugger.Warn($"build delegatte using elapsed time was {stopwatch.ElapsedMilliseconds}.");

            Soldier soldier = NE.ObjectHandler.CreateObject<Soldier>();
            Type soldierType = typeof(Soldier);

            int count = 1000000;

            stopwatch.Reset();
            obj.Health = 100;
            obj2.Health = 150;
            stopwatch.Start();
            for (int n = 0; n < count; ++n)
            {
                d3.DynamicInvoke(obj2);
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier for call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}. obj's health = {obj.Health}, obj2's health = {obj2.Health}.");

            stopwatch.Reset();
            stopwatch.Start();
            ParameterExpression pe = Expression.Parameter(typeof(object));//, "self");
            UnaryExpression ue = Expression.Convert(pe, typeof(Alasijia));
            MethodCallExpression mce = Expression.Call(Expression.Constant(obj), m1, ue);
            Expression<Action<object>> e = Expression.Lambda<Action<object>>(mce, pe);
            Action<object> action1 = e.Compile();

            mce = Expression.Call(Expression.Constant(obj), m2, ue);
            e = Expression.Lambda<Action<object>>(mce, pe);
            Action<object> action2 = e.Compile();

            Action<object> action3 = (Action<object>) Delegate.Combine(action1, action2);
            stopwatch.Stop();
            Debugger.Warn($"build action using elapsed time was {stopwatch.ElapsedMilliseconds}.");

            stopwatch.Reset();
            obj.Health = 100;
            obj2.Health = 150;
            stopwatch.Start();
            for (int n = 0; n < count; ++n)
            {
                //action1.Invoke(obj2);
                //action2.Invoke(obj2);
                action3.Invoke(obj2);
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier for call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}. obj's health = {obj.Health}, obj2's health = {obj2.Health}.");

            stopwatch.Reset();
            obj.Health = 100;
            obj2.Health = 150;
            stopwatch.Start();
            for (int n = 0; n < count; ++n)
            {
                m1.Invoke(obj, new object[] { obj2 });
                m2.Invoke(obj, new object[] { obj2 });
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier for call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}. obj's health = {obj.Health}, obj2's health = {obj2.Health}.");
        }

        public void Test3()
        {
            NE.TimerHandler.Schedule("good", 2000, 10, delegate (int sessionID)
            {
                Debugger.Log("call test3 with session {0}.", sessionID);
            });
        }

        public void Test4()
        {
            Debugger.Log("call test4 ...");
            NE.TimerHandler.Unschedule("good");
            NE.TimerHandler.Schedule("good", 2000, 10, delegate (int sessionID)
            {
                Debugger.Log("call test4 with session {0}.", sessionID);
            });
        }
    }
}
