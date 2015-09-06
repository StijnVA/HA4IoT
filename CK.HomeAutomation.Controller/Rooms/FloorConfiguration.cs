﻿using System;
using CK.HomeAutomation.Actuators;
using CK.HomeAutomation.Hardware.CCTools;
using CK.HomeAutomation.Hardware.Drivers;

namespace CK.HomeAutomation.Controller.Rooms
{
    internal class FloorConfiguration
    {
        public void Setup(Home home, IOBoardManager ioBoardManager, CCToolsFactory ccToolsFactory, TemperatureAndHumiditySensorBridgeDriver sensorBridgeDriver)
        {
            var hsrel5Stairway = ccToolsFactory.CreateHSREL5(Device.StairwayHSREL5, 60);
            var hspe8UpperFloor = ioBoardManager.GetOutputBoard(Device.UpperFloorAndOfficeHSPE8);
            var hspe16_FloorAndLowerBathroom = ccToolsFactory.CreateHSPE16OutputOnly(Device.LowerFloorAndLowerBathroomHSPE16, 17);

            var input1 = ioBoardManager.GetInputBoard(Device.Input1);
            var input2 = ioBoardManager.GetInputBoard(Device.Input2);
            var input4 = ioBoardManager.GetInputBoard(Device.Input4);

            var floor = home.AddRoom(Room.Floor)
                .WithMotionDetector(Floor.StairwayMotionDetector, input2.GetInput(1))
                .WithMotionDetector(Floor.StairsLowerMotionDetector, input4.GetInput(7))
                .WithMotionDetector(Floor.StairsUpperMotionDetector, input4.GetInput(6))
                .WithMotionDetector(Floor.LowerFloorMotionDetector, input1.GetInput(4))
                .WithTemperatureSensor(Floor.LowerFloorTemperatureSensor, 4, sensorBridgeDriver)
                .WithHumiditySensor(Floor.LowerFloorHumiditySensor, 4, sensorBridgeDriver)
                .WithLamp(Floor.LampStairsCeiling1, hspe8UpperFloor.GetOutput(4).WithInvertedState())
                .WithLamp(Floor.LampStairsCeiling2, hspe8UpperFloor.GetOutput(5).WithInvertedState())
                .WithLamp(Floor.LampStairsCeiling3, hspe8UpperFloor.GetOutput(7).WithInvertedState())
                .WithLamp(Floor.LampStairsCeiling4, hspe8UpperFloor.GetOutput(6).WithInvertedState())
                .WithLamp(Floor.Lamp1, hspe16_FloorAndLowerBathroom.GetOutput(5).WithInvertedState())
                .WithLamp(Floor.Lamp2, hspe16_FloorAndLowerBathroom.GetOutput(6).WithInvertedState())
                .WithLamp(Floor.Lamp3, hspe16_FloorAndLowerBathroom.GetOutput(7).WithInvertedState())
                .WithLamp(Floor.StairwayLampCeiling, hsrel5Stairway.GetOutput(0))
                .WithLamp(Floor.StairwayLampWall, hsrel5Stairway.GetOutput(1))
                .WithRollerShutter(Floor.StairwayRollerShutter, hsrel5Stairway.GetOutput(4), hsrel5Stairway.GetOutput(3), RollerShutter.DefaultMaxMovingDuration)
                .WithButton(Floor.ButtonLowerFloorUpper, input1.GetInput(0))
                .WithButton(Floor.ButtonLowerFloorLower, input1.GetInput(5))
                .WithButton(Floor.ButtonLowerFloorAtBathroom, input1.GetInput(1))
                .WithButton(Floor.ButtonLowerFloorAtKitchen, input1.GetInput(3))
                .WithButton(Floor.ButtonStairsLowerUpper, input4.GetInput(4))
                .WithButton(Floor.ButtonStairsLowerLower, input1.GetInput(2))
                .WithButton(Floor.ButtonStairsUpper, input4.GetInput(5))
                .WithButton(Floor.ButtonStairway, input1.GetInput(6))
                .WithLamp(Floor.LampStairs1, hspe16_FloorAndLowerBathroom.GetOutput(8).WithInvertedState())
                .WithLamp(Floor.LampStairs2, hspe16_FloorAndLowerBathroom.GetOutput(9).WithInvertedState())
                .WithLamp(Floor.LampStairs3, hspe16_FloorAndLowerBathroom.GetOutput(10).WithInvertedState())
                .WithLamp(Floor.LampStairs4, hspe16_FloorAndLowerBathroom.GetOutput(11).WithInvertedState())
                .WithLamp(Floor.LampStairs5, hspe16_FloorAndLowerBathroom.GetOutput(13).WithInvertedState())
                .WithLamp(Floor.LampStairs6, hspe16_FloorAndLowerBathroom.GetOutput(12).WithInvertedState());

            floor.CombineActuators(Floor.CombinedStairwayLamp)
                .WithActuator(floor.Lamp(Floor.StairwayLampCeiling))
                .WithActuator(floor.Lamp(Floor.StairwayLampWall));

            floor.SetupAutomaticTurnOnAction()
                .WithMotionDetector(floor.MotionDetector(Floor.StairwayMotionDetector))
                .WithButton(floor.Button(Floor.ButtonStairway))
                .WithTarget(floor.BinaryStateOutputActuator(Floor.CombinedStairwayLamp))
                .WithOnlyAtNightTimeRange(home.WeatherStation)
                .WithOnDuration(TimeSpan.FromSeconds(30));

            floor.CombineActuators(Floor.CombinedLamps)
                .WithActuator(floor.Lamp(Floor.Lamp1))
                .WithActuator(floor.Lamp(Floor.Lamp2))
                .WithActuator(floor.Lamp(Floor.Lamp3));

            floor.SetupAutomaticTurnOnAction()
                .WithMotionDetector(floor.MotionDetector(Floor.LowerFloorMotionDetector))
                .WithButton(floor.Button(Floor.ButtonLowerFloorUpper))
                .WithButton(floor.Button(Floor.ButtonLowerFloorAtBathroom))
                .WithButton(floor.Button(Floor.ButtonLowerFloorAtKitchen))
                .WithTarget(floor.BinaryStateOutputActuator(Floor.CombinedLamps))
                .WithOnlyAtNightTimeRange(home.WeatherStation)
                .WithOnDuration(TimeSpan.FromSeconds(20));

            floor.CombineActuators(Floor.CombinedLampStairsCeiling)
                .WithActuator(floor.Lamp(Floor.LampStairsCeiling1))
                .WithActuator(floor.Lamp(Floor.LampStairsCeiling2))
                .WithActuator(floor.Lamp(Floor.LampStairsCeiling3))
                .WithActuator(floor.Lamp(Floor.LampStairsCeiling4));

            floor.SetupAutomaticTurnOnAction()
                .WithMotionDetector(floor.MotionDetector(Floor.StairsLowerMotionDetector))
                .WithMotionDetector(floor.MotionDetector(Floor.StairsUpperMotionDetector))
                .WithButton(floor.Button(Floor.ButtonStairsUpper))
                .WithTarget(floor.BinaryStateOutputActuator(Floor.CombinedLampStairsCeiling))
                .WithOnDuration(TimeSpan.FromSeconds(20));

            floor.SetupAlwaysOn()
                .WithActuator(floor.Lamp(Floor.LampStairs1))
                .WithActuator(floor.Lamp(Floor.LampStairs2))
                .WithActuator(floor.Lamp(Floor.LampStairs3))
                .WithActuator(floor.Lamp(Floor.LampStairs4))
                .WithActuator(floor.Lamp(Floor.LampStairs5))
                .WithActuator(floor.Lamp(Floor.LampStairs6))
                .WithOnlyAtNightRange(home.WeatherStation)
                .WithOffBetweenRange(TimeSpan.FromHours(23), TimeSpan.FromHours(4));
            
            floor.SetupAutomaticRollerShutters().WithRollerShutter(floor.RollerShutter(Floor.StairwayRollerShutter));
        }

        private enum Floor
        {
            StairwayMotionDetector,
            StairwayTemperatureAndHumiditySensor,

            StairwayLampWall,
            StairwayLampCeiling,
            CombinedStairwayLamp,

            StairwayRollerShutter,

            LowerFloorTemperatureSensor,
            LowerFloorHumiditySensor,
            LowerFloorMotionDetector,

            StairsLowerMotionDetector,
            StairsUpperMotionDetector,

            ButtonLowerFloorUpper,
            ButtonLowerFloorLower,
            ButtonLowerFloorAtBathroom,
            ButtonLowerFloorAtKitchen,
            ButtonStairway,
            ButtonStairsLowerUpper,
            ButtonStairsLowerLower,
            ButtonStairsUpper,

            Lamp1,
            Lamp2,
            Lamp3,
            CombinedLamps,

            LampStairsCeiling1,
            LampStairsCeiling2,
            LampStairsCeiling3,
            LampStairsCeiling4,
            CombinedLampStairsCeiling,

            LampStairs1,
            LampStairs2,
            LampStairs3,
            LampStairs4,
            LampStairs5,
            LampStairs6,
            CombinedLampStairs
        }
    }
}