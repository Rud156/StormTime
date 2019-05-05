#pragma once

namespace  Managers
{
	class SharedTexturesManager
	{
	private:
		static SharedTexturesManager* _instance;
		SharedTexturesManager();

	public:
		enum TextureType
		{
			Background,

			Circle_1,
			Circle_2,
			Circle_3,
			Circle_4,

			Square_1,
			Square_2,
			Square_3,
			Square_4,
		};

		~SharedTexturesManager();
		static  SharedTexturesManager* Instance();

		void loadAllTextures();
		void disposeAllTextures();

		void getTexture(TextureType textureType);
	};
}
