#pragma once

#include <SFML/Graphics.hpp>
#include <vector>

namespace Managers {
	class ControlsManager
	{
	public:
		~ControlsManager();
		static ControlsManager* Instance();

		enum KeyState
		{
			JustPressed,
			Pressed,
			JustReleased,
			Released
		};

		KeyState getKeyState(int keyCode);

		void handleKeyPress(int keyCode);
		void handleKeyReleased(int keyCode);

		void update();

	private:
		struct KeyStatusInfo
		{
			KeyState keyState;
			bool updateOccurred;
		};
		std::vector<KeyStatusInfo> _keyStates;
		
		static ControlsManager* _instance;
		ControlsManager();
	};
}

