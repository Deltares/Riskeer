using System.Collections.Generic;

namespace SharpMap.UI.Tools.Zooming
{
    /// <summary>
    /// Object for keeping track of the zoom history
    /// </summary>
    public class ZoomHistoryTool : ZoomTool
    {
        private ZoomState currentZoomState;
        private readonly Stack<ZoomState> undoStack = new Stack<ZoomState>();
        private readonly Stack<ZoomState> redoStack = new Stack<ZoomState>();
        private bool isZoomChangeTriggeredByNavigation;

        public ZoomHistoryTool()
        {
            Name = "ZoomHistory";
        }

        /// <summary>
        /// Undo last zoom and update redo-stack
        /// </summary>
        public void PreviousZoomState()
        {
            if (undoStack.Count > 0)
            {
                ZoomState previousState = undoStack.Pop();
                if (previousState != null)
                {
                    if (currentZoomState != null)
                    {
                        redoStack.Push(currentZoomState);
                    }
                    isZoomChangeTriggeredByNavigation = true;
                    Map.Center = previousState.Center;
                    Map.Zoom = previousState.Zoom;
                }
            }
        }

        /// <summary>
        /// Redo last zoom and update undo-stack
        /// </summary>
        public void NextZoomState()
        {
            if (redoStack.Count > 0)
            {
                ZoomState nextState = redoStack.Pop();
                if (nextState != null)
                {
                    if (currentZoomState != null)
                    {
                        undoStack.Push(currentZoomState);
                    }
                    isZoomChangeTriggeredByNavigation = true;
                    Map.Center = nextState.Center;
                    Map.Zoom = nextState.Zoom;
                }
            }
        }

        /// <summary>
        /// Number of undo zoom steps that is available
        /// </summary>
        public int UndoCount
        {
            get { return (undoStack.Count); }
        }

        /// <summary>
        /// Number of redo zoom steps that is available
        /// </summary>
        public int RedoCount
        {
            get { return (redoStack.Count); }
        }

        /// <summary>
        /// Used to store current ZoomState if it changes
        /// </summary>
        /// <param name="map"></param>
        public void MapRendered(Map map)
        {
            // More logically this should be done in MapViewOnChange() but that event does not fire
            // when zoom rectangle or zoompan is performed
            if (!isZoomChangeTriggeredByNavigation)
            {
                if ((currentZoomState == null) || !currentZoomState.Zoom.Equals(map.Zoom) || !currentZoomState.Center.Equals(map.Center))
                {
                    if (currentZoomState != null)
                    {
                        undoStack.Push(currentZoomState);
                        redoStack.Clear();
                    }
                }
            }
            isZoomChangeTriggeredByNavigation = false;
            currentZoomState = new ZoomState(map.Zoom, map.Center);
        }
    }
}