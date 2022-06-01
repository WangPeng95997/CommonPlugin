using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonPlugin
{
    public static class MapManager
    {
        public static List<Smod2Room> Rooms { get; private set; }
        public static Smod2Room MircohidRoom { get; private set; }
        public static Smod2Room Scp012Room { get; private set; }
        public static Smod2Room Scp049Room { get; private set; }
        public static Smod2Room Scp079Room { get; private set; }
        public static Smod2Room Scp096Room { get; private set; }
        public static Smod2Room Scp106Room { get; private set; }
        public static Smod2Room Scp173Room { get; private set; }
        public static Smod2Room Scp703Room { get; private set; }
        public static Smod2Room Scp939Room { get; private set; }
        public static Smod2Room ServersRoom { get; private set; }

        static MapManager() => Rooms = new List<Smod2Room>();
        public static void GetRooms()
        {
            Rooms.Clear();
            Rooms.AddRange(GameObject.FindGameObjectsWithTag("Room").Select(r => new Smod2Room(r.name, r.transform, r.transform.position)));

            Transform transfrom;
            const string PocketDimensionRoom = "HeavyRooms/PocketWorld";
            transfrom = GameObject.Find(PocketDimensionRoom).transform;
            Rooms.Add(new Smod2Room(PocketDimensionRoom, transfrom, transfrom.position));

            const string surfaceRoom = "Outside";
            transfrom = GameObject.Find(surfaceRoom).transform;
            Rooms.Add(new Smod2Room(surfaceRoom, transfrom, transfrom.position));

            foreach (Smod2Room smod2Room in Rooms)
            {
                int bracketStart = smod2Room.Roomname.IndexOf('(') - 1;
                if (bracketStart > 0)
                    smod2Room.Roomname = smod2Room.Roomname.Remove(bracketStart, smod2Room.Roomname.Length - bracketStart);

                switch (smod2Room.Roomname)
                {
                    // Lcz SCP-012
                    case "LCZ_012":
                        Scp012Room = smod2Room;
                        Scp012Room.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y - 2.5f, smod2Room.Position.z);
                        Scp012Room.Position = new Vector3(smod2Room.Position.x, smod2Room.Position.y - 2.5f, smod2Room.Position.z);
                        break;

                    // Lcz Scp703Room
                    case "LCZ_Cafe":
                        Scp703Room = smod2Room;
                        Scp703Room.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y + 0.25f, smod2Room.Position.z);
                        Scp703Room.Position = new Vector3(smod2Room.Position.x, smod2Room.Position.y + 0.25f, smod2Room.Position.z);
                        break;

                    // Hcz Scp079Room
                    case "HCZ_079":
                        Scp079Room = smod2Room;
                        Scp079Room.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y + 2.5f, smod2Room.Position.z);
                        Scp079Room.Position = new Vector3(smod2Room.Position.x, smod2Room.Position.y + 2.5f, smod2Room.Position.z);
                        break;

                    // Hcz Scp106Room
                    case "HCZ_106":
                        Scp106Room = smod2Room;
                        Scp106Room.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y + 2.5f, smod2Room.Position.z);
                        Scp106Room.Position = new Vector3(smod2Room.Position.x, smod2Room.Position.y + 2.5f, smod2Room.Position.z);
                        break;

                    // Hcz MircoHIDRoom
                    case "HCZ_Hid":
                        MircohidRoom = smod2Room;
                        MircohidRoom.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y + 2.5f, smod2Room.Position.z);
                        MircohidRoom.Position = new Vector3(smod2Room.Position.x, smod2Room.Position.y + 2.5f, smod2Room.Position.z);
                        break;

                    // Hcz ServersRoom
                    case "HCZ_Servers":
                        ServersRoom = smod2Room;
                        ServersRoom.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y + 2.5f, smod2Room.Position.z);
                        ServersRoom.Position = new Vector3(smod2Room.Position.x, smod2Room.Position.y + 2.5f, smod2Room.Position.z);
                        break;

                    // Hcz Scp939Room
                    case "HCZ_Testroom":
                        Scp939Room = smod2Room;
                        Scp939Room.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y - 12.5f, smod2Room.Position.z);
                        Scp939Room.Position = new Vector3(smod2Room.Position.x, smod2Room.Position.y - 12.5f, smod2Room.Position.z);
                        break;
                }
            }

            Vector vector;
            Map map = PluginManager.Manager.Server.Map;

            // Hcz Scp049Room
            vector = map.GetRandomSpawnPoint(Smod2.API.RoleType.SCP_049);
            MapManager.Scp049Room = new Smod2Room("Scp049Room");
            MapManager.Scp049Room.Position = new Vector3(vector.x, vector.y + 0.2f, vector.z);
            MapManager.Scp049Room.Position2 = new Vector(vector.x, vector.y + 0.2f, vector.z);

            // Hcz Scp096Room
            vector = map.GetRandomSpawnPoint(Smod2.API.RoleType.SCP_096);
            MapManager.Scp096Room = new Smod2Room("Scp096Room");
            MapManager.Scp096Room.Position = new Vector3(vector.x, vector.y + 0.2f, vector.z);
            MapManager.Scp096Room.Position2 = new Vector(vector.x, vector.y + 0.2f, vector.z);

            // Lcz Scp173Room
            vector = map.GetRandomSpawnPoint(Smod2.API.RoleType.SCP_173);
            MapManager.Scp173Room = new Smod2Room("Scp173Room");
            MapManager.Scp173Room.Position = new Vector3(vector.x, vector.y + 0.2f, vector.z);
            MapManager.Scp173Room.Position2 = new Vector(vector.x, vector.y + 0.2f, vector.z);
        }
    }

    public class Smod2Room
    {
        public string Roomname { get; set; }
        public Vector3 Position { get; set; }
        public Vector Position2 { get; set; }
        public Transform Transform { get; set; }
        

        public Smod2Room(string roomname)
        {
            this.Roomname = roomname;
        }

        public Smod2Room(string roomname, Transform transform, Vector3 position)
        {
            this.Roomname = roomname;
            this.Position = position;
            this.Transform = transform;
        }
    }
}