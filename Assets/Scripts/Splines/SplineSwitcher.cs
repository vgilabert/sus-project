using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System;

public class SplineSwitcher : MonoBehaviour
    {
        private SplineTracer splineTracer;
        private double _lastPercent = 0.0;
        //private Wagon _wagon;

        void Start()
        {
            splineTracer = GetComponent<SplineTracer>();
            //Subscribe to the onNode event to receive junction information automatically when a Node is passed
            splineTracer.onNode += OnJunction;

            //If the tracer is a SplineFollower (which should be the the case), subscribe to onEndReached and onOnBeginningReached
            if (splineTracer is SplineFollower)
            {
                SplineFollower follower = (SplineFollower)splineTracer;
                follower.onBeginningReached += FollowerOnBeginningReached;
                follower.onEndReached += FollowerOnEndReached;
            }
        }
        
        private void FollowerOnBeginningReached(double lastPercent)
        {
            _lastPercent = lastPercent;
        }

        private void FollowerOnEndReached(double lastPercent)
        {
            _lastPercent = lastPercent;
        }

        //Called when the tracer has passed a junction (a Node)
        private void OnJunction(List<SplineTracer.NodeConnection> passed)
        {
            Node node = passed[0].node; //Get the node of the junction
            NodeSwitch junctionSwitch = node.GetComponent<NodeSwitch>(); //Look for a JunctionSwitch component
            if (junctionSwitch == null) return; //No JunctionSwitch - ignore it - this isn't a real junction
            if (junctionSwitch.bridges.Length == 0) return; //The JunctionSwitch does not have bridge elements
            foreach (NodeSwitch.Bridge bridge in junctionSwitch.bridges)
            {
                //Look for a suitable bridge element based on the spline we are currently traversing
                if (!bridge.active) continue;
                if (bridge.a == bridge.b) continue; //Skip bridge if it points to the same spline  
                int currentConnection = 0;
                Node.Connection[] connections = node.GetConnections();
                //get the connected splines and find the index of the tracer's current spline
                for (int i = 0; i < connections.Length; i++)
                {
                    if (connections[i].spline == splineTracer.spline)
                    {
                        currentConnection = i;
                        break;
                    }
                }
                //Skip the bridge if we are not on one of the splines that the switch connects
                if (currentConnection != bridge.a && currentConnection != bridge.b) continue;
                if (currentConnection == bridge.a)
                {
                    if ((int)splineTracer.direction != (int)bridge.bDirection) continue;
                    //This bridge is suitable and should use it
                    SwitchSpline(connections[bridge.a], connections[bridge.b]);
                    return;
                }
                else
                {
                    if ((int)splineTracer.direction != (int)bridge.aDirection) continue;
                    //This bridge is suitable and should use it
                    SwitchSpline(connections[bridge.b], connections[bridge.a]);
                    return;
                }
            }
        }

        void SwitchSpline(Node.Connection from, Node.Connection to)
        {
            //See how much units we have travelled past that Node in the last frame
           
            float excessDistance = from.spline.CalculateLength(from.spline.GetPointPercent(from.pointIndex), splineTracer.UnclipPercent(_lastPercent));
            //Set the spline to the tracer
            splineTracer.spline = to.spline;
            splineTracer.RebuildImmediate();
            //Get the location of the junction point in percent along the new spline
            double startpercent = splineTracer.ClipPercent(to.spline.GetPointPercent(to.pointIndex));
            if (Vector3.Dot(from.spline.Evaluate(from.pointIndex).forward, to.spline.Evaluate(to.pointIndex).forward) < 0f)
            {
                if (splineTracer.direction == Spline.Direction.Forward) splineTracer.direction = Spline.Direction.Backward;
                else splineTracer.direction = Spline.Direction.Forward;
            }
            //Position the tracer at the new location and travel excessDistance along the new spline
            splineTracer.SetPercent(splineTracer.Travel(startpercent, excessDistance, splineTracer.direction));
        }
    }
