mergeInto(LibraryManager.library, {

	AddMoneyExtern: function (value1, value2) {
		ysdk.adv.showRewardedVideo({
    callbacks: {
        onOpen: () => {
          console.log('Video ad open.');
          myGameInstance.SendMessage("Button-VideoAds", "MuteAudio");
        },
        onRewarded: () => {
          console.log('Rewarded!');
          myGameInstance.SendMessage("Button-VideoAds", "AddMoneyOrTime", value1 + "," + value2);
        },
        onClose: () => {
          console.log('Video ad closed.');
          myGameInstance.SendMessage("Button-VideoAds", "UnmuteAudio");
        }, 
        onError: (e) => {
          console.log('Error while open video ad:', e);
          myGameInstance.SendMessage("Button-VideoAds", "UnmuteAudio");
        }
    }
})
    }
});