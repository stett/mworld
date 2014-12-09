using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld
{
    /// <summary>
    /// A single vertex for a path
    /// </summary>
    public class PartPathVertex
    {
        public Vector2 pos;
        public PartPathVertex connection;
        private PartPath path;
        public PartPathVertex next;
        public PartPathVertex prev;

        public PartPathVertex(PartPath _path)
        {
            path = _path;
            pos = new Vector2();
            connection = null;
        }
        public PartPathVertex(PartPath _path, Vector2 _pos)
        {
            path = _path;
            pos = _pos;
            connection = null;
        }
        public PartPathVertex(PartPath _path, float x, float y)
        {
            path = _path;
            pos = new Vector2(x, y);
            connection = null;
        }

        public PartPath get_path() { return path; }
        public Vector3 pos3() { return new Vector3(pos.X, pos.Y, 0); }
        public bool is_first() { return path.first_vert() == this; }
        public bool is_last() { return path.last_vert() == this; }

        public float get_length()
        {
            if (next == null) return 0;
            else return (next.pos - pos).Length();
        }

        public Vector2 get_n()
        {
            if (next == null) return Vector2.Zero;
            else return Vector2.Normalize(next.pos - pos);
        }

        public Vector2 w_pos()
        {
            Vector3 w_pos3 = Vector3.Transform(pos3(), path.get_part().get_trans());
            return new Vector2(w_pos3.X, w_pos3.Y);
        }
    }

    /// <summary>
    /// A path contains a list of vertices
    /// </summary>
    public class PartPath
    {
        private Part part;
        private List<PartPathVertex> vertices;
        private float length;

        public PartPath(Part _part)
        {
            part = _part;
            vertices = new List<PartPathVertex>();
            length = 0f;
        }

        public void clear_connections()
        {
            foreach (PartPathVertex vert in vertices)
            {
                vert.connection = null;
            }
        }

        public Part get_part() { return part; }
        public PartPathVertex get_vert(int i) { return vertices[i]; }
        public int num_verts() { return vertices.Count; }
        public PartPathVertex first_vert() { if (vertices.Count > 0) return vertices[0]; else return null; }
        public PartPathVertex last_vert() { if (vertices.Count > 0) return vertices[vertices.Count - 1]; else return null; }
        public float get_length() { return length; }

        public void add_vert(PartPathVertex vert) {
            if (vertices.Count > 0) {
                vert.next = null;
                vert.prev = last_vert();
                last_vert().next = vert;
            }
            vertices.Add(vert);
            length = 0f;
            PartPathVertex prev_vert = null;
            foreach (PartPathVertex _vert in vertices)
            {
                if (prev_vert != null)
                {
                    length += (vert.pos - prev_vert.pos).Length();
                }
                prev_vert = _vert;
            }
        }

        public void clear_verts()
        {
            foreach (PartPathVertex vert in vertices)
                PartPath.disconnect_vert(vert);
            vertices.Clear();
            length = 0f;
        }

        public void disconnect_verts()
        {
            foreach (PartPathVertex vert in vertices)
                disconnect_vert(vert);
        }

        public static void connect_verts(PotentialConnection connection)
        {
            connect_verts(connection.vert_0, connection.vert_1);
        }
        public static void connect_verts(PartPathVertex vert_a, PartPathVertex vert_b)
        {
            //if (vert_a.get_path().get_part() == vert_b.get_path().get_part()) return;

            if (vert_a.connection != null)
            {
                vert_a.connection.connection = null;
                vert_a.connection = null;
            }

            if (vert_b.connection != null)
            {
                vert_b.connection.connection = null;
                vert_b.connection = null;
            }

            vert_a.connection = vert_b;
            vert_b.connection = vert_a;
        }

        public static void disconnect_vert(PartPathVertex vert, bool ignore_internal_verts=true)
        {
            if (vert.connection != null)
            {
                if (ignore_internal_verts && vert.get_path().get_part() == vert.connection.get_path().get_part()) return;

                vert.connection.connection = null;
                vert.connection = null;
            }
        }
    }

    public class PotentialConnection
    {
        public PartPathVertex vert_0;
        public PartPathVertex vert_1;
        public PotentialConnection(PartPathVertex _vert_0, PartPathVertex _vert_1)
        {
            vert_0 = _vert_0;
            vert_1 = _vert_1;
        }
    }

    /// <summary>
    /// The base class for all game components.
    /// The simplest possible component is just a single node.
    /// </summary>
    public class Part
    {
        //
        // Members
        //

        public List<PartPath> paths;
        public Vector2 pos;
        public float angle;
        protected float r;
        protected List<Part> connections;
        protected Matrix transformation;
        protected bool draw_intermediate_nodes;
        protected string _name;
        protected bool _can_rotate;
        protected bool _can_edit_endpoints;

        //
        // 'Tors
        //

        public Part()
        {
            connections = new List<Part>();
            paths = new List<PartPath>();
            pos = Vector2.Zero;
            angle = 0.0f;
            r = 50.0f;
            draw_intermediate_nodes = true;

            _name = "part";
            _can_rotate = true;
            _can_edit_endpoints = false;
        }


        //
        // Basic Methods
        //

        public Vector3 pos3()
        {
            return new Vector3(pos.X, pos.Y, 0);
        }

        public Matrix get_trans()
        {
            return transformation;
        }

        public float get_r()
        {
            return r;
        }

        public bool is_selected()
        {
            return MWorld.selected_part.part == this;
        }

        public void disconnect_verts()
        {
            foreach (PartPath path in paths)
                path.disconnect_verts();
        }

        public void connect_part(Part connection)
        {
            foreach (Part part in connections)
                if (part == connection) return;
            connections.Add(connection);
        }

        public void disconnect_part(Part connection)
        {
            while (connections.Remove(connection));
        }

        public bool is_connected_to_part(Part connection)
        {
            foreach (Part part in connections)
                if (part == connection) return true;
            return false;
        }


        //
        // Virtual Methods
        //

        public virtual Part new_copy(Part part = null)
        {
            if (part == null) part = new Part();
            foreach (PartPath path in paths)
            {
                PartPath new_path = new PartPath(this);
                for (int i = 0; i < path.num_verts(); i++)
                    new_path.add_vert(new PartPathVertex(new_path, path.get_vert(i).pos));
            }
            part.pos = pos;
            part.angle = angle;

            return part;
        }

        public virtual void update(float dt)
        {
            transformation = Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(pos3());
        }

        public virtual void draw(Color color)
        {
            // Draw vertices
            if (MWorld.mode == MWorld.GameMode.Edit)
            {
                foreach (PartPath path in paths)
                {
                    for (int i = 0; i < path.num_verts(); i++)
                    {

                        // Get the the vertex, and the vectors representing its pos in screen coordinates
                        PartPathVertex vert = path.get_vert(i);
                        Vector3 v0 = Vector3.Transform(vert.pos3(), transformation);

                        // If this isn't the last vertex, get the next one and draw a line between them
                        if (i < path.num_verts() - 1)
                        {
                            Vector3 v1 = Vector3.Transform(path.get_vert(i + 1).pos3(), transformation);
                            Graphics.draw_line(v0, v1, Color.OrangeRed, 1f, false);
                        }

                        if (vert == path.first_vert())
                        {
                            Graphics.draw_tex("node", vert.w_pos(), MWorld.hud_rotation * 5f, .5f / MWorld.view.get_scale(), Color.OrangeRed, 1f);
                        }
                        else if (vert == path.last_vert())
                        {
                            float extra_scale = 1.2f;
                            if (vert.connection != null) extra_scale = 1f;
                            Graphics.draw_circle(vert.w_pos(), Graphics.get_tex("node").Width * extra_scale * .25f / MWorld.view.get_scale(), Color.OrangeRed, 1f, false);
                            Graphics.draw_tex("node-inner", vert.w_pos(), MWorld.hud_rotation * 5f, .5f / (MWorld.view.get_scale() * extra_scale), Color.OrangeRed, 1f);
                        }
                        else if (draw_intermediate_nodes)
                        {
                            Graphics.draw_tex("node", vert.w_pos(), (MWorld.hud_rotation + angle) * 5f, .3f / MWorld.view.get_scale(), Color.OrangeRed, 1f);
                        }
                    }
                }
            }

            // Draw connections
            foreach (Part connection in connections)
            {
                Vector2 diff = connection.pos - pos;
                Vector2 n = diff;
                n.Normalize();
                Vector2 a = pos + n * (r + 5 / MWorld.view.get_scale());
                Vector2 b = connection.pos - n * (connection.r + 15 / MWorld.view.get_scale());
                Graphics.draw_arrow(a, b, Color.Salmon, 10f);
            }
        }

        protected void draw_paths(Color color, float width)
        {
            foreach (PartPath path in paths)
            {
                for (int i = 0; i < path.num_verts(); i++)
                {
                    // Get the the vertex, and the vectors representing its pos in screen coordinates
                    PartPathVertex vertex = path.get_vert(i);
                    Vector3 v0 = Vector3.Transform(path.get_vert(i).pos3(), transformation);

                    // If this isn't the last vertex, get the next one and draw a line between them
                    if (i < path.num_verts() - 1)
                    {
                        Vector3 v1 = Vector3.Transform(path.get_vert(i + 1).pos3(), transformation);
                        Graphics.draw_line(v0, v1, color, 20, true, .6f);
                    }
                }
            }
        }

        public virtual void receive()
        {
        }

        public virtual void send()
        {
            foreach (Part part in connections)
                part.receive();
        }

        public virtual void update_path_pos(ref PartPathVertex vert, ref float dist, ref float vel, out Vector2 path_pos, out Vector2 path_tan)
        {
            update_path_pos(ref vert, ref dist, ref vel, vel, out path_pos, out path_tan);
        }
        public virtual void update_path_pos(ref PartPathVertex vert, ref float dist, ref float vel, float vel_step, out Vector2 path_pos, out Vector2 path_tan)
        {
            // We're going to move past the beginning of this vertex
            if (dist + vel < 0)
            {
                // Move to the previous vert on this path
                if (vert.prev != null)
                {
                    // Calculate the remaining step velocity and the remaining total velocity
                    float vel_coef = Vector2.Dot(vert.prev.get_n(), vert.get_n());
                    vel_step += dist;
                    vel_step *= vel_coef;
                    vel *= vel_coef;

                    //
                    vert = vert.prev;
                    dist = vert.get_length();

                    //
                    update_path_pos(ref vert, ref dist, ref vel, vel_step, out path_pos, out path_tan);
                    return;
                }

                // Move to the previous path connected to this vertex
                else if (vert.connection != null)
                {
                    //
                    Vector2 n0 = vert.connection.w_pos() - vert.connection.prev.w_pos();
                    Vector2 n1 = vert.next.w_pos() - vert.w_pos();
                    n0.Normalize();
                    n1.Normalize();
                    float vel_coef = Vector2.Dot(n0, n1);
                    vel_step += dist;
                    vel_step *= vel_coef;
                    vel *= vel_coef;

                    //
                    vert = vert.connection.prev;
                    dist = vert.get_length();

                    //
                    update_path_pos(ref vert, ref dist, ref vel, vel_step, out path_pos, out path_tan);
                    return;
                }

                // We've reached the beginning of the path
                else
                {
                    vel = 0;
                    dist = 0;
                }
            }

            // We're going to move past the end of the current vertex's length
            else if (dist + vel > vert.get_length())
            {
                // Move to the next vert on this path
                if (vert.next.next != null)
                {
                    // Calculate the remaining step velocity and the remaining total velocity
                    float vel_coef = Vector2.Dot(vert.get_n(), vert.next.get_n());
                    vel_step -= vert.get_length() - dist;
                    vel_step *= vel_coef;
                    vel *= vel_coef;

                    //
                    vert = vert.next;
                    dist = 0;

                    //
                    update_path_pos(ref vert, ref dist, ref vel, vel_step, out path_pos, out path_tan);
                    return;
                }

                // Move to the next path connected to this vertex
                else if (vert.next.connection != null)
                {
                    //
                    Vector2 n0 = vert.next.w_pos() - vert.w_pos();
                    Vector2 n1 = vert.next.connection.next.w_pos() - vert.next.connection.w_pos();
                    n0.Normalize();
                    n1.Normalize();
                    float vel_coef = Vector2.Dot(n0, n1);
                    vel_step -= vert.get_length();
                    vel_step *= vel_coef;
                    vel *= vel_coef;

                    //
                    vert = vert.next.connection;
                    dist = 0;

                    //
                    vert.get_path().get_part().update_path_pos(ref vert, ref dist, ref vel, vel_step, out path_pos, out path_tan);
                    return;
                }

                // We've reached the end of the path
                else
                {
                    vel = 0;
                    dist = vert.get_length();
                }
            }

            // We're safely within this vertex's length, so just update the vert_dist.
            else
            {
                dist += vel;
            }

            path_tan = vert.next.w_pos() - vert.w_pos();
            path_tan.Normalize();
            path_pos = vert.w_pos() + path_tan * dist;
        }

        public bool can_rotate() { return _can_rotate; }
        public bool can_edit_endpoints() { return _can_edit_endpoints; }
        public string name() { return _name; }
    }


    /// <summary>
    /// A spinning ring which highlights selected or hovered parts
    /// </summary>
    public class PartSelector
    {
        //
        // Members
        //

        public Part part;
        float t_trans;
        float v_trans;
        Vector2 pos;
        float r_max;


        //
        // 'Tors
        //

        public PartSelector()
        {
            pos = Vector2.Zero;
            part = null;
            t_trans = 0.0f;
            v_trans = 0.1f;
        }


        //
        // Methods
        //

        public void update(float dt)
        {
            //angle += v_angle; // * dt;
            //while (angle > 2f * (float)Math.PI) angle -= 2f * (float)Math.PI;

            if (part != null)
            {
                pos = part.pos;
                r_max = part.get_r();
                if (t_trans < 1f) t_trans += v_trans; // * dt;
                else t_trans = 1f;
            }
            else
            {
                if (t_trans > 0f) t_trans -= v_trans; // * dt;
                else t_trans = 0f;
            }
        }

        public void draw(Color color)
        {
            if (color == null) color = Color.White;
            float scale_max = 2 * r_max / (Graphics.get_tex("selector").Width - 2 * 24);
            float scale = Tween.smoothstep(1.5f * scale_max, scale_max, t_trans);
            float alpha = Tween.smoothstep(0, 1, t_trans);
            if (alpha > 0f)
            {
                Graphics.draw_tex("selector", pos, MWorld.hud_rotation, scale, color, alpha);
                if (part != null)
                {
                    Graphics.draw_string(part.name(), pos, -MWorld.view.get_angle(), color, alpha, 1);
                }
            }
        }
    }
}