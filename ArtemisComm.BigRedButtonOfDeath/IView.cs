using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtemisComm.BigRedButtonOfDeath
{
    public interface IView : IDisposable
    {
        string Host { get; }
        int Port { get; }


        void AlertAboutArtemisVersionConflict(string message);

        /// <summary>
        /// Gets the ship selection.
        /// </summary>
        /// <param name="shipList">The ship list.  Return index # of array (0-based).</param>
        /// <returns></returns>
        int GetShipSelection(PlayerShip[] shipList);
        event EventHandler ConnectRequested;

        bool RedAlertEnabled { get; set; }
        bool ShieldsRaised { get; set; }

        event EventHandler StartSelfDestruct;
        event EventHandler CancelSelfDestruct;
        event EventHandler DisconnectRequested;
        event EventHandler Disposing;

        void ConnectionLostWarning();
        void ConnectionFailed();

        void GameStarted();
        void GameEnded();
        void SimulationEnded();

    }
}
