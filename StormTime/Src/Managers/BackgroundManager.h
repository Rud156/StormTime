#pragma once
#include <SFML/Graphics/Texture.hpp>

namespace Managers {
	class BackgroundManager
	{
	private:
		const float _movementVelocityChangeRate = 600;
		sf::Texture _backgroundTextures;

		float _y1, _y2, _y3, _y4;
		float _x1, _x2, _x3, _x4;

		float _textureWidth;
		float _textureHeight;

		float _currentSpeed;
		float _maxSpeed;

		bool _isBackgroundScrolling;

		void setLeftAndDownMovement();
		void setUpAndRightMovement();

	public:
		void update(float deltaTime, float playerRotationAngle);

		void activateScrolling();
		void deActivateScrolling();
	};
}
