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

		ControlsManager::KeyState getKeyState(int keyCode);

		void handleKeyPress(int keyCode);
		void handleKeyReleased(int keyCode);

		void update();

	private:
		std::vector<ControlsManager::KeyState> _keyStates;

		static ControlsManager* _instance;
		ControlsManager();
	};
}

