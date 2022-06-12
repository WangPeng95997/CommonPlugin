using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects;
using MapGeneration;
using UnityEngine;
using Smod2;
using Smod2.API;
using NorthwoodLib.Pools;

namespace CommonPlugin
{
    public static class MapManager
    {
        public static List<Smod2Room> Rooms { get; private set; }
        public static Smod2Room MircohidRoom { get; private set; }
        public static Smod2Room Scp330Room { get; private set; }
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

            List<GameObject> roomObjects = ListPool<GameObject>.Shared.Rent(Object.FindObjectsOfType<RoomIdentifier>().Select(x => x.gameObject));

            foreach (GameObject roomObject in roomObjects)
                Rooms.Add(new Smod2Room(roomObject.name, roomObject.transform, roomObject.transform.position));

            ListPool<GameObject>.Shared.Return(roomObjects);

            foreach (Smod2Room smod2Room in Rooms)
            {
                int bracketStart = smod2Room.Roomname.IndexOf('(') - 1;
                if (bracketStart > 0)
                    smod2Room.Roomname = smod2Room.Roomname.Remove(bracketStart, smod2Room.Roomname.Length - bracketStart);

                switch (smod2Room.Roomname)
                {
                    // Scp079Room
                    case "HCZ_079":
                        Scp079Room = smod2Room;
                        Scp079Room.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y, smod2Room.Position.z) + Vector.Up;
                        Scp079Room.Position = smod2Room.Position + Vector3.up;
                        break;

                    // Scp106Room
                    case "HCZ_106":
                        Scp106Room = smod2Room;
                        Scp106Room.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y, smod2Room.Position.z) + Vector.Up;
                        Scp106Room.Position = smod2Room.Position + Vector3.up;
                        break;

                    // Scp703Room
                    case "LCZ_Cafe":
                        Scp703Room = smod2Room;
                        Scp703Room.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y, smod2Room.Position.z) + Vector.Up;
                        Scp703Room.Position = smod2Room.Position + Vector3.up;
                        break;

                    // Scp939Room
                    case "HCZ_Testroom":
                        Scp939Room = smod2Room;
                        Scp939Room.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y, smod2Room.Position.z) - Vector.Up * 14.5f;
                        Scp939Room.Position = smod2Room.Position - Vector3.up * 14.5f;
                        break;

                    // MircoHIDRoom
                    case "HCZ_Hid":
                        MircohidRoom = smod2Room;
                        MircohidRoom.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y, smod2Room.Position.z) + Vector.Up;
                        MircohidRoom.Position = smod2Room.Position + Vector3.up;
                        break;

                    // ServersRoom
                    case "HCZ_Servers":
                        ServersRoom = smod2Room;
                        ServersRoom.Position2 = new Vector(smod2Room.Position.x, smod2Room.Position.y, smod2Room.Position.z) + Vector.Up;
                        ServersRoom.Position = smod2Room.Position + Vector3.up;
                        break;
                }
            }

            // Scp049Room
            GameObject gameObject = GameObject.FindGameObjectWithTag("SP_049");
            Vector3 position = gameObject.transform.position;
            Scp049Room = new Smod2Room("Scp049Room");
            Scp049Room.Transform = gameObject.transform;
            Scp049Room.Position2 = new Vector(position.x, position.y, position.z);
            Scp049Room.Position = position;

            // Scp096Room
            gameObject = GameObject.FindGameObjectWithTag("SCP_096");
            position = gameObject.transform.position;
            Scp096Room = new Smod2Room("Scp096Room");
            Scp096Room.Transform = gameObject.transform;
            Scp096Room.Position2 = new Vector(position.x, position.y, position.z);
            Scp096Room.Position = position;

            // Scp173Room
            gameObject = GameObject.FindGameObjectWithTag("SP_173");
            position = gameObject.transform.position;
            Scp173Room = new Smod2Room("Scp173Room");
            Scp173Room.Transform = gameObject.transform;
            Scp173Room.Position2 = new Vector(position.x, position.y, position.z);
            Scp173Room.Position = position;

            // Scp330Room
            Scp330Interobject scp330Interobject = Object.FindObjectOfType<Scp330Interobject>();
            position = scp330Interobject.transform.position;
            Scp330Room = new Smod2Room("Scp330Room");
            Scp330Room.Transform = scp330Interobject.transform;
            Scp330Room.Position2 = new Vector(position.x, position.y, position.z) + Vector.Up;
            Scp330Room.Position = position + Vector3.up;
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