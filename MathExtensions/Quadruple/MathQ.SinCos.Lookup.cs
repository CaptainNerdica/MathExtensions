using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExtensions
{
	partial class MathQ
	{
		partial class SinCosLookup
		{
			internal static readonly Quadruple[] SinCosTable = new Quadruple[]
			{
				/* x =  1.48437500000000000000000000000000000e-01L 3ffc3000000000000000000000000000 */
				/* cos(x) = 0.fd2f5320e1b790209b4dda2f98f79caaa7b873aff1014b0fbc5243766d03cb006bc837c4358 */
				new Quadruple(0x3ffe_fa5e_a641_c36f, 0x2041_369b_b45f_31ef),
				new Quadruple(0x3f8b_caaa_7b87_3aff, 0x1014_b0fb_c524_3767),
				/* sin(x) = 0.25dc50bc95711d0d9787d108fd438cf5959ee0bfb7a1e36e8b1a112968f356657420e9cc9ea */
				new Quadruple(0x3ffc_2ee2_85e4_ab88, 0xe86c_bc3e_8847_ea1c),
				new Quadruple(0x3f8a_9eb2_b3dc_17f6, 0xf43c_6dd1_6342_252d),

				/* x = 1.56250000000000000000000000000000000e-01 3ffc4000000000000000000000000000 */
				/* cos(x) = 0.fce1a053e621438b6d60c76e8c45bf0a9dc71aa16f922acc10e95144ec796a249813c9cb649 */
				new Quadruple(0x3ffe_f9c3_40a7_cc42, 0x8716_dac1_8edd_188b),
				new Quadruple(0x3f8c_f854_ee38_d50b, 0x7c91_5660_874a_8a27),
				/* sin(x) = 0.27d66258bacd96a3eb335b365c87d59438c5142bb56a489e9b8db9d36234ffdebb6bdc22d8e */
				new Quadruple(0x3ffc_3eb3_12c5_d66c, 0xb51f_599a_d9b2_e43f),
				new Quadruple(0xbf8a_4d78_e75d_7a89, 0x52b6_ec2c_8e48_c594),

				/* x = 1.64062500000000000000000000000000000e-01 3ffc5000000000000000000000000000 */
				/* cos(x) = 0.fc8ffa01ba6807417e05962b0d9fdf1fddb0cc4c07d22e19e08019bffa50a6c7acdb40307a3 */
				new Quadruple(0x3ffe_f91f_f403_74d0, 0x0e82_fc0b_2c56_1b40),
				new Quadruple(0xbf8c_0701_1279_9d9f, 0xc16e_8f30_fbff_3200),
				/* sin(x) = 0.29cfd49b8be4f665276cab01cbf0426934906c3dd105473b226e410b1450f62e53ff7c6cce1 */
				new Quadruple(0x3ffc_4e7e_a4dc_5f27, 0xb329_3b65_580e_5f82),
				new Quadruple(0x3f88_349a_4836_1ee8, 0x82a3_9d91_3720_858a),

				/* x = 1.71875000000000000000000000000000000e-01 3ffc6000000000000000000000000000 */
				/* cos(x) = 0.fc3a6170f767ac735d63d99a9d439e1db5e59d3ef153a4265d5855850ed82b536bf361b80e3 */
				new Quadruple(0x3ffe_f874_c2e1_eecf, 0x58e6_bac7_b335_3a87),
				new Quadruple(0x3f8b_e1db_5e59_d3ef, 0x153a_4265_d585_5851),
				/* sin(x) = 0.2bc89f9f424de5485de7ce03b2514952b9faf5648c3244d4736feb95dbb9da49f3b58a9253b */
				new Quadruple(0x3ffc_5e44_fcfa_126f, 0x2a42_ef3e_701d_928a),
				new Quadruple(0x3f8a_2a57_3f5e_ac91, 0x8648_9a8e_6dfd_72bb),

				/* x = 1.79687500000000000000000000000000000e-01 3ffc7000000000000000000000000000 */
				/* cos(x) = 0.fbe0d7f7fef11e70aa43b8abf4f6a457cea20c8f3f676b47781f9821bbe9ce04b3c7b981c0b */
				new Quadruple(0x3ffe_f7c1_afef_fde2, 0x3ce1_5487_7157_e9ed),
				new Quadruple(0x3f8c_22be_7510_6479, 0xfb3b_5a3b_c0fc_c10e),
				/* sin(x) = 0.2dc0bb80b49a97ffb34e8dd1f8db9df7af47ed2dcf58b12c8e7827e048cae929da02c04ecac */
				new Quadruple(0x3ffc_6e05_dc05_a4d4, 0xbffd_9a74_6e8f_c6dd),
				new Quadruple(0xbf88_0428_5c09_6918, 0x53a7_69b8_c3ec_0fdc),

				/* x = 1.87500000000000000000000000000000000e-01 3ffc8000000000000000000000000000 */
				/* cos(x) = 0.fb835efcf670dd2ce6fe7924697eea13ea358867e9cdb3899b783f4f9f43aa5626e8b67b3bc */
				new Quadruple(0x3ffe_f706_bdf9_ece1, 0xba59_cdfc_f248_d2fe),
				new Quadruple(0xbf8b_5ec1_5ca7_7981, 0x6324_c766_487c_0b06),
				/* sin(x) = 0.2fb8205f75e56a2b56a1c4792f856258769af396e0189ef72c05e4df59a6b00e4b44a6ea515 */
				new Quadruple(0x3ffc_7dc1_02fb_af2b, 0x515a_b50e_23c9_7c2b),
				new Quadruple(0x3f88_2c3b_4d79_cb70, 0x0c4f_7b96_02f2_6fad),

				/* x = 1.95312500000000000000000000000000000e-01 3ffc9000000000000000000000000000 */
				/* cos(x) = 0.fb21f7f5c156696b00ac1fe28ac5fd76674a92b4df80d9c8a46c684399005deccc41386257c */
				new Quadruple(0x3ffe_f643_efeb_82ac, 0xd2d6_0158_3fc5_158c),
				new Quadruple(0xbf88_44cc_5ab6_a590, 0x3f93_1bad_c9cb_de34),
				/* sin(x) = 0.31aec65df552876f82ece9a2356713246eba6799983d7011b0b3698d6e1da919c15d57c30c1 */
				new Quadruple(0x3ffc_8d76_32ef_aa94, 0x3b7c_1767_4d11_ab39),
				new Quadruple(0xbf8a_9b72_28b3_0ccc, 0xf851_fdc9_e992_ce52),

				/* x = 2.03125000000000000000000000000000000e-01 3ffca000000000000000000000000000 */
				/* cos(x) = 0.fabca467fb3cb8f1d069f01d8ea33ade5bfd68296ecd1cc9f7b7609bbcf3676e726c3301334 */
				new Quadruple(0x3ffe_f579_48cf_f679, 0x71e3_a0d3_e03b_1d46),
				new Quadruple(0x3f8c_d6f2_dfeb_414b, 0x7668_e64f_bdbb_04de),
				/* sin(x) = 0.33a4a5a19d86246710f602c44df4fa513f4639ce938477aeeabb82e8e0a7ed583a188879fd4 */
				new Quadruple(0x3ffc_9d25_2d0c_ec31, 0x2338_87b0_1622_6fa8),
				new Quadruple(0xbf89_6bb0_2e71_8c5b, 0x1ee2_1445_511f_45c8),

				/* x = 2.10937500000000000000000000000000000e-01 3ffcb000000000000000000000000000 */
				/* cos(x) = 0.fa5365e8f1d3ca27be1db5d76ae64d983d7470a4ab0f4ccf65a2b8c67a380df949953a09bc1 */
				new Quadruple(0x3ffe_f4a6_cbd1_e3a7, 0x944f_7c3b_6bae_d5cd),
				new Quadruple(0xbf8c_933e_145c_7ada, 0xa785_9984_d2ea_39cc),
				/* sin(x) = 0.3599b652f40ec999df12a0a4c8561de159c98d4e54555de518b97f48886f715d8df5f4f093e */
				new Quadruple(0x3ffc_accd_b297_a076, 0x4cce_f895_0526_42b1),
				new Quadruple(0xbf88_0f53_1b39_58d5, 0xd551_0d73_a340_5bbc),

				/* x = 2.18750000000000000000000000000000000e-01 3ffcc000000000000000000000000000 */
				/* cos(x) = 0.f9e63e1d9e8b6f6f2e296bae5b5ed9c11fd7fa2fe11e09fc7bde901abed24b6365e72f7db4e */
				new Quadruple(0x3ffe_f3cc_7c3b_3d16, 0xdede_5c52_d75c_b6be),
				new Quadruple(0xbf8c_31f7_0140_2e80, 0xf70f_b01c_210b_7f2a),
				/* sin(x) = 0.378df09db8c332ce0d2b53d865582e4526ea336c768f68c32b496c6d11c1cd241bb9f1da523 */
				new Quadruple(0x3ffc_bc6f_84ed_c619, 0x9670_695a_9ec3_2ac1),
				new Quadruple(0x3f8a_c8a4_dd46_6d8e, 0xd1ed_1865_692d_8da2),

				/* x = 2.26562500000000000000000000000000000e-01 3ffcd000000000000000000000000000 */
				/* cos(x) = 0.f9752eba9fff6b98842beadab054a932fb0f8d5b875ae63d6b2288d09b148921aeb6e52f61b */
				new Quadruple(0x3ffe_f2ea_5d75_3ffe, 0xd731_0857_d5b5_60a9),
				new Quadruple(0x3f8c_4997_d87c_6adc, 0x3ad7_31eb_5914_4685),
				/* sin(x) = 0.39814cb10513453cb97b21bc1ca6a337b150c21a675ab85503bc09a436a10ab1473934e20c8 */
				new Quadruple(0x3ffc_cc0a_6588_289a, 0x29e5_cbd9_0de0_e535),
				new Quadruple(0x3f88_9bd8_a861_0d33, 0xad5c_2a81_de04_d21b),

				/* x = 2.34375000000000000000000000000000000e-01 3ffce000000000000000000000000000 */
				/* cos(x) = 0.f90039843324f9b940416c1984b6cbed1fc733d97354d4265788a86150493ce657cae032674 */
				new Quadruple(0x3ffe_f200_7308_6649, 0xf372_8082_d833_096e),
				new Quadruple(0xbf8c_a097_01c6_6134, 0x6559_5ecd_43ba_bcf5),
				/* sin(x) = 0.3b73c2bf6b4b9f668ef9499c81f0d965087f1753fa64b086e58cb8470515c18c1412f8c2e02 */
				new Quadruple(0x3ffc_db9e_15fb_5a5c, 0xfb34_77ca_4ce4_0f87),
				new Quadruple(0xbf89_a6bd_e03a_2b01, 0x66d3_de46_9cd1_ee3f),

				/* x = 2.42187500000000000000000000000000000e-01 3ffcf000000000000000000000000000 */
				/* cos(x) = 0.f887604e2c39dbb20e4ec5825059a789ffc95b275ad9954078ba8a28d3fcfe9cc2c1d49697b */
				new Quadruple(0x3ffe_f10e_c09c_5873, 0xb764_1c9d_8b04_a0b3),
				new Quadruple(0x3f8c_3c4f_fe4a_d93a, 0xd6cc_aa03_c5d4_5147),
				/* sin(x) = 0.3d654aff15cb457a0fca854698aba33039a8a40626609204472d9d40309b626eccc6dff0ffa */
				new Quadruple(0x3ffc_eb2a_57f8_ae5a, 0x2bd0_7e54_2a34_c55d),
				new Quadruple(0x3f88_981c_d452_0313, 0x3049_0223_96ce_a018),

				/* x = 2.50000000000000000000000000000000000e-01 3ffd0000000000000000000000000000 */
				/* cos(x) = 0.f80aa4fbef750ba783d33cb95f94f8a41426dbe79edc4a023ef9ec13c944551c0795b84fee1 */
				new Quadruple(0x3ffe_f015_49f7_deea, 0x174f_07a6_7972_bf2a),
				new Quadruple(0xbf89_d6fa_f649_0618, 0x48ed_7f70_4184_fb0e),
				/* sin(x) = 0.3f55dda9e62aed7513bd7b8e6a3d1635dd5676648d7db525898d7086af9330f03c7f285442a */
				new Quadruple(0x3ffc_faae_ed4f_3157, 0x6ba8_9deb_dc73_51e9),
				new Quadruple(0xbf8a_3944_5531_336e, 0x5049_5b4e_ce51_ef2a),

				/* x = 2.57812500000000000000000000000000000e-01 3ffd0800000000000000000000000000 */
				/* cos(x) = 0.f78a098069792daabc9ee42591b7c5a68cb1ab822aeb446b3311b4ba5371b8970e2c1547ad7 */
				new Quadruple(0x3ffe_ef14_1300_d2f2, 0x5b55_793d_c84b_2370),
				new Quadruple(0xbf8c_d2cb_9a72_a3ee, 0xa8a5_dca6_6772_5a2d),
				/* sin(x) = 0.414572fd94556e6473d620271388dd47c0ba050cdb5270112e3e370e8c4705ae006426fb5d5 */
				new Quadruple(0x3ffd_0515_cbf6_5155, 0xb991_cf58_809c_4e23),
				new Quadruple(0x3f8b_d47c_0ba0_50cd, 0xb527_0112_e3e3_70e9),

				/* x = 2.65625000000000000000000000000000000e-01 3ffd1000000000000000000000000000 */
				/* cos(x) = 0.f7058fde0788dfc805b8fe88789e4f4253e3c50afe8b22f41159620ab5940ff7df9557c0d1f */
				new Quadruple(0x3ffe_ee0b_1fbc_0f11, 0xbf90_0b71_fd10_f13d),
				new Quadruple(0xbf8c_85ed_60e1_d7a8, 0x0ba6_e85f_7534_efaa),
				/* sin(x) = 0.4334033bcd90d6604f5f36c1d4b84451a87150438275b77470b50e5b968fa7962b5ffb379b7 */
				new Quadruple(0x3ffd_0cd0_0cef_3643, 0x5981_3d7c_db07_52e1),
				new Quadruple(0x3f89_146a_1c54_10e0, 0x9d6d_dd1c_2d43_96e6),

				/* x = 2.73437500000000000000000000000000000e-01 3ffd1800000000000000000000000000 */
				/* cos(x) = 0.f67d3a26af7d07aa4bd6d42af8c0067fefb96d5b46c031eff53627f215ea3242edc3f2e13eb */
				new Quadruple(0x3ffe_ecfa_744d_5efa, 0x0f54_97ad_a855_f180),
				new Quadruple(0x3f89_9ffb_ee5b_56d1, 0xb00c_7bfd_4d89_fc85),
				/* sin(x) = 0.452186aa5377ab20bbf2524f52e3a06a969f47166ab88cf88c111ad12c55941021ef3317a1a */
				new Quadruple(0x3ffd_1486_1aa9_4dde, 0xac82_efc9_493d_4b8f),
				new Quadruple(0xbf8b_f956_960b_8e99, 0x5477_3077_3eee_52ed),

				/* x = 2.81250000000000000000000000000000000e-01 3ffd2000000000000000000000000000 */
				/* cos(x) = 0.f5f10a7bb77d3dfa0c1da8b57842783280d01ce3c0f82bae3b9d623c168d2e7c29977994451 */
				new Quadruple(0x3ffe_ebe2_14f7_6efa, 0x7bf4_183b_516a_f085),
				new Quadruple(0xbf89_f35f_cbf8_c70f, 0xc1f5_1471_18a7_70fa),
				/* sin(x) = 0.470df5931ae1d946076fe0dcff47fe31bb2ede618ebc607821f8462b639e1f4298b5ae87fd3 */
				new Quadruple(0x3ffd_1c37_d64c_6b87, 0x6518_1dbf_8373_fd20),
				new Quadruple(0xbf87_ce44_d121_9e71, 0x439f_87de_07b9_d49c),

				/* x = 2.89062500000000000000000000000000000e-01 3ffd2800000000000000000000000000 */
				/* cos(x) = 0.f561030ddd7a78960ea9f4a32c6521554995667f5547bafee9ec48b3155cdb0f7fd00509713 */
				new Quadruple(0x3ffe_eac2_061b_baf4, 0xf12c_1d53_e946_58ca),
				new Quadruple(0x3f8c_0aaa_4cab_33fa, 0xaa3d_d7f7_4f62_4599),
				/* sin(x) = 0.48f948446abcd6b0f7fccb100e7a1b26eccad880b0d24b59948c7cdd49514d44b933e6985c2 */
				new Quadruple(0x3ffd_23e5_2111_aaf3, 0x5ac3_dff3_2c40_39e8),
				new Quadruple(0x3f8b_b26e_ccad_880b, 0x0d24_b599_48c7_cdd5),

				/* x = 2.96875000000000000000000000000000000e-01 3ffd3000000000000000000000000000 */
				/* cos(x) = 0.f4cd261d3e6c15bb369c8758630d2ac00b7ace2a51c0631bfeb39ed158ba924cc91e259c195 */
				new Quadruple(0x3ffe_e99a_4c3a_7cd8, 0x2b76_6d39_0eb0_c61a),
				new Quadruple(0x3f8c_5600_5bd6_7152, 0x8e03_18df_f59c_f68b),
				/* sin(x) = 0.4ae37710fad27c8aa9c4cf96c03519b9ce07dc08a1471775499f05c29f86190aaebaeb9716e */
				new Quadruple(0x3ffd_2b8d_dc43_eb49, 0xf22a_a713_3e5b_00d4),
				new Quadruple(0x3f8b_9b9c_e07d_c08a, 0x1471_7754_99f0_5c2a),

				/* x = 3.04687500000000000000000000000000000e-01 3ffd3800000000000000000000000000 */
				/* cos(x) = 0.f43575f94d4f6b272f5fb76b14d2a64ab52df1ee8ddf7c651034e5b2889305a9ea9015d758a */
				new Quadruple(0x3ffe_e86a_ebf2_9a9e, 0xd64e_5ebf_6ed6_29a5),
				new Quadruple(0x3f8c_3255_a96f_8f74, 0x6efb_e328_81a7_2d94),
				/* sin(x) = 0.4ccc7a50127e1de0cb6b40c302c651f7bded4f9e7702b0471ae0288d091a37391950907202f */
				new Quadruple(0x3ffd_3331_e940_49f8, 0x7783_2dad_030c_0b19),
				new Quadruple(0x3f8b_1f7b_ded4_f9e7, 0x702b_0471_ae02_88d1),

				/* x = 3.12500000000000000000000000000000000e-01 3ffd4000000000000000000000000000 */
				/* cos(x) = 0.f399f500c9e9fd37ae9957263dab8877102beb569f101ee4495350868e5847d181d50d3cca2 */
				new Quadruple(0x3ffe_e733_ea01_93d3, 0xfa6f_5d32_ae4c_7b57),
				new Quadruple(0x3f8a_0ee2_057d_6ad3, 0xe203_dc89_2a6a_10d2),
				/* sin(x) = 0.4eb44a5da74f600207aaa090f0734e288603ffadb3eb2542a46977b105f8547128036dcf7f0 */
				new Quadruple(0x3ffd_3ad1_2976_9d3d, 0x8008_1eaa_8243_c1cd),
				new Quadruple(0x3f8a_c510_c07f_f5b6, 0x7d64_a854_8d2e_f621),

				/* x = 3.20312500000000000000000000000000000e-01 3ffd4800000000000000000000000000 */
				/* cos(x) = 0.f2faa5a1b74e82fd61fa05f9177380e8e69b7b15a945e8e5ae1124bf3d12b0617e03af4fab5 */
				new Quadruple(0x3ffe_e5f5_4b43_6e9d, 0x05fa_c3f4_0bf2_2ee7),
				new Quadruple(0x3f86_d1cd_36f6_2b52, 0x8bd1_cb5c_2249_7e7a),
				/* sin(x) = 0.509adf9a7b9a5a0f638a8fa3a60a199418859f18b37169a644fdb986c21ecb00133853bc35b */
				new Quadruple(0x3ffd_426b_7e69_ee69, 0x683d_8e2a_3e8e_9828),
				new Quadruple(0x3f8b_9941_8859_f18b, 0x3716_9a64_4fdb_986c),

				/* x = 3.28125000000000000000000000000000000e-01 3ffd5000000000000000000000000000 */
				/* cos(x) = 0.f2578a595224dd2e6bfa2eb2f99cc674f5ea6f479eae2eb580186897ae3f893df1113ca06b8 */
				new Quadruple(0x3ffe_e4af_14b2_a449, 0xba5c_d7f4_5d65_f33a),
				new Quadruple(0xbf8c_cc58_50ac_85c3, 0x0a8e_8a53_ff3c_bb43),
				/* sin(x) = 0.5280326c3cf481823ba6bb08eac82c2093f2bce3c4eb4ee3dec7df41c92c8a4226098616075 */
				new Quadruple(0x3ffd_4a00_c9b0_f3d2, 0x0608_ee9a_ec23_ab21),
				new Quadruple(0xbf8b_3df6_c0d4_31c3, 0xb14b_11c2_1382_0be3),

				/* x = 3.35937500000000000000000000000000000e-01 3ffd5800000000000000000000000000 */
				/* cos(x) = 0.f1b0a5b406b526d886c55feadc8d0dcc8eb9ae2ac707051771b48e05b25b000009660bdb3e3 */
				new Quadruple(0x3ffe_e361_4b68_0d6a, 0x4db1_0d8a_bfd5_b91a),
				new Quadruple(0x3f8a_b991_d735_c558, 0xe0e0_a2ee_3691_c0b6),
				/* sin(x) = 0.54643b3da29de9b357155eef0f332fb3e66c83bf4dddd9491c5eb8e103ccd92d6175220ed51 */
				new Quadruple(0x3ffd_5190_ecf6_8a77, 0xa6cd_5c55_7bbc_3ccd),
				new Quadruple(0xbf8b_04c1_9937_c40b, 0x2222_6b6e_3a14_71f0),

				/* x = 3.43750000000000000000000000000000000e-01 3ffd6000000000000000000000000000 */
				/* cos(x) = 0.f105fa4d66b607a67d44e042725204435142ac8ad54dfb0907a4f6b56b06d98ee60f19e557a */
				new Quadruple(0x3ffe_e20b_f49a_cd6c, 0x0f4c_fa89_c084_e4a4),
				new Quadruple(0x3f89_10d4_50ab_22b5, 0x537e_c241_e93d_ad5b),
				/* sin(x) = 0.5646f27e8bd65cbe3a5d61ff06572290ee826d9674a00246b05ae26753cdfc90d9ce81a7d02 */
				new Quadruple(0x3ffd_591b_c9fa_2f59, 0x72f8_e975_87fc_195d),
				new Quadruple(0xbf8b_d6f1_17d9_2698, 0xb5ff_db94_fa51_d98b),

				/* x = 3.51562500000000000000000000000000000e-01 3ffd6800000000000000000000000000 */
				/* cos(x) = 0.f0578ad01ede707fa39c09dc6b984afef74f3dc8d0efb0f4c5a6b13771145b3e0446fe33887 */
				new Quadruple(0x3ffe_e0af_15a0_3dbc, 0xe0ff_4738_13b8_d731),
				new Quadruple(0xbf8c_a808_4586_11b9, 0x7882_7859_d2ca_7644),
				/* sin(x) = 0.582850a41e1dd46c7f602ea244cdbbbfcdfa8f3189be794dda427ce090b5f85164f1f80ac13 */
				new Quadruple(0x3ffd_60a1_4290_7877, 0x51b1_fd80_ba89_1337),
				new Quadruple(0xbf89_100c_815c_339d, 0x9061_ac89_6f60_c7dc),

				/* x = 3.59375000000000000000000000000000000e-01 3ffd7000000000000000000000000000 */
				/* cos(x) = 0.efa559f5ec3aec3a4eb03319278a2d41fcf9189462261125fe6147b078f1daa0b06750a1654 */
				new Quadruple(0x3ffe_df4a_b3eb_d875, 0xd874_9d60_6632_4f14),
				new Quadruple(0x3f8c_6a0f_e7c8_c4a3, 0x1130_892f_f30a_3d84),
				/* sin(x) = 0.5a084e28e35fda2776dfdbbb5531d74ced2b5d17c0b1afc4647529d50c295e36d8ceec126c1 */
				new Quadruple(0x3ffd_6821_38a3_8d7f, 0x689d_db7f_6eed_54c7),
				new Quadruple(0x3f8b_74ce_d2b5_d17c, 0x0b1a_fc46_4752_9d51),

				/* x = 3.67187500000000000000000000000000000e-01 3ffd7800000000000000000000000000 */
				/* cos(x) = 0.eeef6a879146af0bf9b95ea2ea0ac0d3e2e4d7e15d93f48cbd41bf8e4fded40bef69e19eafa */
				new Quadruple(0x3ffe_ddde_d50f_228d, 0x5e17_f372_bd45_d416),
				new Quadruple(0xbf8c_f960_e8d9_40f5, 0x1360_5b9a_15f2_038e),
				/* sin(x) = 0.5be6e38ce8095542bc14ee9da0d36483e6734bcab2e07624188af5653f114eeb46738fa899d */
				new Quadruple(0x3ffd_6f9b_8e33_a025, 0x550a_f053_ba76_834e),
				new Quadruple(0xbf8b_b7c1_98cb_4354, 0xd1f8_9dbe_7750_a9ac),

				/* x = 3.75000000000000000000000000000000000e-01 3ffd8000000000000000000000000000 */
				/* cos(x) = 0.ee35bf5ccac89052cd91ddb734d3a47e262e3b609db604e217053803be0091e76daf28a89b7 */
				new Quadruple(0x3ffe_dc6b_7eb9_9591, 0x20a5_9b23_bb6e_69a7),
				new Quadruple(0x3f8c_23f1_3171_db04, 0xedb0_2710_b829_c01e),
				/* sin(x) = 0.5dc40955d9084f48a94675a2498de5d851320ff5528a6afb3f2e24de240fce6cbed1ba0ccd6 */
				new Quadruple(0x3ffd_7710_2557_6421, 0x3d22_a519_d689_2638),
				new Quadruple(0xbf8b_a27a_ecdf_00aa, 0xd759_504c_0d1d_b21e),

				/* x = 3.82812500000000000000000000000000000e-01 3ffd8800000000000000000000000000 */
				/* cos(x) = 0.ed785b5c44741b4493c56bcb9d338a151c6f6b85d8f8aca658b28572c162b199680eb9304da */
				new Quadruple(0x3ffe_daf0_b6b8_88e8, 0x3689_278a_d797_3a67),
				new Quadruple(0x3f8a_42a3_8ded_70bb, 0x1f15_94cb_1650_ae58),
				/* sin(x) = 0.5f9fb80f21b53649c432540a50e22c53057ff42ae0fdf1307760dc0093f99c8efeb2fbd7073 */
				new Quadruple(0x3ffd_7e7e_e03c_86d4, 0xd927_10c9_5029_4389),
				new Quadruple(0xbf8b_3acf_a800_bd51, 0xf020_ecf8_89f2_3ff7),

				/* x = 3.90625000000000000000000000000000000e-01 3ffd9000000000000000000000000000 */
				/* cos(x) = 0.ecb7417b8d4ee3fec37aba4073aa48f1f14666006fb431d9671303c8100d10190ec8179c41d */
				new Quadruple(0x3ffe_d96e_82f7_1a9d, 0xc7fd_86f5_7480_e755),
				new Quadruple(0xbf8c_b870_75cc_cffc, 0x825e_7134_c767_e1bf),
				/* sin(x) = 0.6179e84a09a5258a40e9b5face03e525f8b5753cd0105d93fe6298010c3458e84d75fe420e9 */
				new Quadruple(0x3ffd_85e7_a128_2694, 0x9629_03a6_d7eb_3810),
				new Quadruple(0xbf8b_ada0_74a8_ac32, 0xfefa_26c0_19d6_7fef),

				/* x = 3.98437500000000000000000000000000000e-01 3ffd9800000000000000000000000000 */
				/* cos(x) = 0.ebf274bf0bda4f62447e56a093626798d3013b5942b1abfd155aacc9dc5c6d0806a20d6b9c1 */
				new Quadruple(0x3ffe_d7e4_e97e_17b4, 0x9ec4_88fc_ad41_26c5),
				new Quadruple(0xbf8b_8672_cfec_4a6b, 0xd4e5_402e_aa55_3362),
				/* sin(x) = 0.6352929dd264bd44a02ea766325d8aa8bd9695fc8def3caefba5b94c9a3c873f7b2d3776ead */
				new Quadruple(0x3ffd_8d4a_4a77_4992, 0xf512_80ba_9d98_c976),
				new Quadruple(0x3f8a_5517_b2d2_bf91, 0xbde7_95df_74b7_2993),

				/* x = 4.06250000000000000000000000000000000e-01 3ffda000000000000000000000000000 */
				/* cos(x) = 0.eb29f839f201fd13b93796827916a78f15c85230a4e8ea4b21558265a14367e1abb4c30695a */
				new Quadruple(0x3ffe_d653_f073_e403, 0xfa27_726f_2d04_f22d),
				new Quadruple(0x3f8c_3c78_ae42_9185, 0x2747_5259_0aac_132d),
				/* sin(x) = 0.6529afa7d51b129631ec197c0a840a11d7dc5368b0a47956feb285caa8371c4637ef17ef01b */
				new Quadruple(0x3ffd_94a6_be9f_546c, 0x4a58_c7b0_65f0_2a10),
				new Quadruple(0x3f8a_423a_fb8a_6d16, 0x148f_2adf_d650_b955),

				/* x = 4.14062500000000000000000000000000000e-01 3ffda800000000000000000000000000 */
				/* cos(x) = 0.ea5dcf0e30cf03e6976ef0b1ec26515fba47383855c3b4055a99b5e86824b2cd1a691fdca7b */
				new Quadruple(0x3ffe_d4bb_9e1c_619e, 0x07cd_2edd_e163_d84d),
				new Quadruple(0xbf8c_7502_2dc6_3e3d, 0x51e2_5fd5_2b32_50bd),
				/* sin(x) = 0.66ff380ba0144109e39a320b0a3fa5fd65ea0585bcbf9b1a769a9b0334576c658139e1a1cbe */
				new Quadruple(0x3ffd_9bfc_e02e_8051, 0x0427_8e68_c82c_28ff),
				new Quadruple(0xbf8b_a029_a15f_a7a4, 0x3406_4e58_9656_4fcd),

				/* x = 4.21875000000000000000000000000000000e-01 3ffdb000000000000000000000000000 */
				/* cos(x) = 0.e98dfc6c6be031e60dd3089cbdd18a75b1f6b2c1e97f79225202f03dbea45b07a5ec4efc062 */
				new Quadruple(0x3ffe_d31b_f8d8_d7c0, 0x63cc_1ba6_1139_7ba3),
				new Quadruple(0x3f8a_4eb6_3ed6_583d, 0x2fef_244a_405e_07b8),
				/* sin(x) = 0.68d32473143327973bc712bcc4ccddc47630d755850c0655243b205934dc49ffed8eb76adcb */
				new Quadruple(0x3ffd_a34c_91cc_50cc, 0x9e5c_ef1c_4af3_1333),
				new Quadruple(0x3f8b_dc47_630d_7558, 0x50c0_6552_43b2_0593),

				/* x = 4.29687500000000000000000000000000000e-01 3ffdb800000000000000000000000000 */
				/* cos(x) = 0.e8ba8393eca7821aa563d83491b6101189b3b101c3677f73d7bad7c10f9ee02b7ab4009739a */
				new Quadruple(0x3ffe_d175_0727_d94f, 0x0435_4ac7_b069_236c),
				new Quadruple(0x3f8b_0118_9b3b_101c, 0x3677_f73d_7bad_7c11),
				/* sin(x) = 0.6aa56d8e8249db4eb60a761fe3f9e559be456b9e13349ca99b0bfb787f22b95db3b70179615 */
				new Quadruple(0x3ffd_aa95_b63a_0927, 0x6d3a_d829_d87f_8fe8),
				new Quadruple(0xbf8b_aa64_1ba9_461e, 0xccb6_3566_4f40_4878),

				/* x = 4.37500000000000000000000000000000000e-01 3ffdc000000000000000000000000000 */
				/* cos(x) = 0.e7e367d2956cfb16b6aa11e5419cd0057f5c132a6455bf064297e6a76fe2b72bb630d6d50ff */
				new Quadruple(0x3ffe_cfc6_cfa5_2ad9, 0xf62d_6d54_23ca_833a),
				new Quadruple(0xbf8c_7fd4_051f_66ac, 0xdd52_07cd_eb40_cac5),
				/* sin(x) = 0.6c760c14c8585a51dbd34660ae6c52ac7036a0b40887a0b63724f8b4414348c3063a637f457 */
				new Quadruple(0x3ffd_b1d8_3053_2161, 0x6947_6f4d_1982_b9b1),
				new Quadruple(0x3f8b_2ac7_036a_0b40, 0x887a_0b63_724f_8b44),

				/* x = 4.45312500000000000000000000000000000e-01 3ffdc800000000000000000000000000 */
				/* cos(x) = 0.e708ac84d4172a3e2737662213429e14021074d7e702e77d72a8f1101a7e70410df8273e9aa */
				new Quadruple(0x3ffe_ce11_5909_a82e, 0x547c_4e6e_cc44_2685),
				new Quadruple(0x3f8b_e140_2107_4d7e, 0x702e_77d7_2a8f_1102),
				/* sin(x) = 0.6e44f8c36eb10a1c752d093c00f4d47ba446ac4c215d26b0316442f168459e677d06e7249e3 */
				new Quadruple(0x3ffd_b913_e30d_bac4, 0x2871_d4b4_24f0_03d3),
				new Quadruple(0x3f8b_47ba_446a_c4c2, 0x15d2_6b03_1644_2f17),

				/* x = 4.53125000000000000000000000000000000e-01 3ffdd000000000000000000000000000 */
				/* cos(x) = 0.e62a551594b970a770b15d41d4c0e483e47aca550111df6966f9e7ac3a94ae49e6a71eb031e */
				new Quadruple(0x3ffe_cc54_aa2b_2972, 0xe14e_e162_ba83_a982),
				new Quadruple(0xbf8b_b7c1_b853_5aaf, 0xeee2_0969_9061_853c),
				/* sin(x) = 0.70122c5ec5028c8cff33abf4fd340ccc382e038379b09cf04f9a52692b10b72586060cbb001 */
				new Quadruple(0x3ffd_c048_b17b_140a, 0x3233_fcce_afd3_f4d0),
				new Quadruple(0x3f8a_9987_05c0_706f, 0x3613_9e09_f34a_4d25),

				/* x = 4.60937500000000000000000000000000000e-01 3ffdd800000000000000000000000000 */
				/* cos(x) = 0.e54864fe33e8575cabf5bd0e5cf1b1a8bc7c0d5f61702450fa6b6539735820dd2603ae355d5 */
				new Quadruple(0x3ffe_ca90_c9fc_67d0, 0xaeb9_57eb_7a1c_b9e3),
				new Quadruple(0x3f8c_8d45_e3e0_6afb, 0x0b81_2287_d35b_29cc),
				/* sin(x) = 0.71dd9fb1ff4677853acb970a9f6729c6e3aac247b1c57cea66c77413f1f98e8b9e98e49d851 */
				new Quadruple(0x3ffd_c776_7ec7_fd19, 0xde14_eb2e_5c2a_7d9d),
				new Quadruple(0xbf8b_6391_c553_db84, 0xe3a8_3159_9388_bec1),

				/* x = 4.68750000000000000000000000000000000e-01 3ffde000000000000000000000000000 */
				/* cos(x) = 0.e462dfc670d421ab3d1a15901228f146a0547011202bf5ab01f914431859aef577966bc4fa4 */
				new Quadruple(0x3ffe_c8c5_bf8c_e1a8, 0x4356_7a34_2b20_2452),
				new Quadruple(0xbf8a_d72b_f571_fddb, 0xfa81_4a9f_c0dd_779d),
				/* sin(x) = 0.73a74b8f52947b681baf6928eb3fb021769bf4779bad0e3aa9b1cdb75ec60aad9fc63ff19d5 */
				new Quadruple(0x3ffd_ce9d_2e3d_4a51, 0xeda0_6ebd_a4a3_acff),
				new Quadruple(0xbf8a_fbd1_2c81_710c, 0x8a5e_38aa_c9c6_4914),

				/* x = 4.76562500000000000000000000000000000e-01 3ffde800000000000000000000000000 */
				/* cos(x) = 0.e379c9045f29d517c4808aa497c2057b2b3d109e76c0dc302d4d0698b36e3f0bdbf33d8e952 */
				new Quadruple(0x3ffe_c6f3_9208_be53, 0xaa2f_8901_1549_2f84),
				new Quadruple(0x3f89_5eca_cf44_279d, 0xb037_0c0b_5341_a62d),
				/* sin(x) = 0.756f28d011d98528a44a75fc29c779bd734ecdfb582fdb74b68a4c4c4be54cfd0b2d3ad292f */
				new Quadruple(0x3ffd_d5bc_a340_4766, 0x14a2_9129_d7f0_a71e),
				new Quadruple(0xbf89_90a3_2c4c_8129, 0xf409_22d2_5d6c_eced),

				/* x = 4.84375000000000000000000000000000000e-01 3ffdf000000000000000000000000000 */
				/* cos(x) = 0.e28d245c58baef72225e232abc003c4366acd9eb4fc2808c2ab7fe7676cf512ac7f945ae5fb */
				new Quadruple(0x3ffe_c51a_48b8_b175, 0xdee4_44bc_4655_7800),
				new Quadruple(0x3f8c_e21b_3566_cf5a, 0x7e14_0461_55bf_f3b4),
				/* sin(x) = 0.77353054ca72690d4c6e171fd99e6b39fa8e1ede5f052fd2964534c75340970a3a9cd3c5c32 */
				new Quadruple(0x3ffd_dcd4_c153_29c9, 0xa435_31b8_5c7f_667a),
				new Quadruple(0xbf8b_4c60_571e_121a, 0x0fad_02d6_9bac_b38b),

				/* x = 4.92187500000000000000000000000000000e-01 3ffdf800000000000000000000000000 */
				/* cos(x) = 0.e19cf580eeec046aa1422fa74807ecefb2a1911c94e7b5f20a00f70022d940193691e5bd790 */
				new Quadruple(0x3ffe_c339_eb01_ddd8, 0x08d5_4284_5f4e_9010),
				new Quadruple(0xbf8b_3104_d5e6_ee36, 0xb184_a0df_5ff0_8ffe),
				/* sin(x) = 0.78f95b0560a9a3bd6df7bd981dc38c61224d08bc20631ea932e605e53b579e9e0767dfcbbcb */
				new Quadruple(0x3ffd_e3e5_6c15_82a6, 0x8ef5_b7de_f660_770e),
				new Quadruple(0x3f8a_8c24_49a1_1784, 0x0c63_d526_5cc0_bca7),

				/* x = 5.00000000000000000000000000000000000e-01 3ffe0000000000000000000000000000 */
				/* cos(x) = 0.e0a94032dbea7cedbddd9da2fafad98556566b3a89f43eabd72350af3e8b19e801204d8fe2e */
				new Quadruple(0x3ffe_c152_8065_b7d4, 0xf9db_7bbb_3b45_f5f6),
				new Quadruple(0xbf8c_33d5_4d4c_a62b, 0xb05e_0aa1_46e5_7a86),
				/* sin(x) = 0.7abba1d12c17bfa1d92f0d93f60ded9992f45b4fcaf13cd58b303693d2a0db47db35ae8a3a9 */
				new Quadruple(0x3ffd_eaee_8744_b05e, 0xfe87_64bc_364f_d838),
				new Quadruple(0xbf8b_2666_d0ba_4b03, 0x50ec_32a7_4cfc_96c3),

				/* x = 5.07812500000000000000000000000000000e-01 3ffe0400000000000000000000000000 */
				/* cos(x) = 0.dfb20840f3a9b36f7ae2c515342890b5ec583b8366cc2b55029e95094d31112383f2553498b */
				new Quadruple(0x3ffe_bf64_1081_e753, 0x66de_f5c5_8a2a_6851),
				new Quadruple(0x3f8b_0b5e_c583_b836, 0x6cc2_b550_29e9_5095),
				/* sin(x) = 0.7c7bfdaf13e5ed17212f8a7525bfb113aba6c0741b5362bb8d59282a850b63716bca0c910f0 */
				new Quadruple(0x3ffd_f1ef_f6bc_4f97, 0xb45c_84be_29d4_96ff),
				new Quadruple(0xbf8a_dd8a_8b27_f17c, 0x9593_a88e_54da_faaf),

				/* x = 5.15625000000000000000000000000000000e-01 3ffe0800000000000000000000000000 */
				/* cos(x) = 0.deb7518814a7a931bbcc88c109cd41c50bf8bb48f20ae8c36628d1d3d57574f7dc58f27d91c */
				new Quadruple(0x3ffe_bd6e_a310_294f, 0x5263_7799_1182_139b),
				new Quadruple(0xbf8c_f1d7_a03a_25b8, 0x6fa8_b9e4_ceb9_7161),
				/* sin(x) = 0.7e3a679daaf25c676542bcb4028d0964172961c921823a4ef0c3a9070d886dbd073f6283699 */
				new Quadruple(0x3ffd_f8e9_9e76_abc9, 0x719d_950a_f2d0_0a34),
				new Quadruple(0x3f8a_2c82_e52c_3924, 0x3047_49de_1875_20e2),

				/* x = 5.23437500000000000000000000000000000e-01 3ffe0c00000000000000000000000000 */
				/* cos(x) = 0.ddb91ff318799172bd2452d0a3889f5169c64a0094bcf0b8aa7dcf0d7640a2eba68955a80be */
				new Quadruple(0x3ffe_bb72_3fe6_30f3, 0x22e5_7a48_a5a1_4711),
				new Quadruple(0x3f8b_f516_9c64_a009, 0x4bcf_0b8a_a7dc_f0d7),
				/* sin(x) = 0.7ff6d8a34bd5e8fa54c97482db5159df1f24e8038419c0b448b9eea8939b5d4dfcf40900257 */
				new Quadruple(0x3ffd_ffdb_628d_2f57, 0xa3e9_5325_d20b_6d45),
				new Quadruple(0x3f8b_9df1_f24e_8038, 0x419c_0b44_8b9e_ea89),

				/* x = 5.31250000000000000000000000000000000e-01 3ffe1000000000000000000000000000 */
				/* cos(x) = 0.dcb7777ac420705168f31e3eb780ce9c939ecada62843b54522f5407eb7f21e556059fcd734 */
				new Quadruple(0x3ffe_b96e_eef5_8840, 0xe0a2_d1e6_3c7d_6f02),
				new Quadruple(0xbf8c_8b1b_6309_a92c, 0xebde_255d_6e85_5fc1),
				/* sin(x) = 0.81b149ce34caa5a4e650f8d09fd4d6aa74206c32ca951a93074c83b2d294d25dbb0f7fdfad2 */
				new Quadruple(0x3ffe_0362_939c_6995, 0x4b49_cca1_f1a1_3faa),
				new Quadruple(0xbf8c_4aac_5efc_9e69, 0xab57_2b67_c59b_e269),

				/* x = 5.39062500000000000000000000000000000e-01 3ffe1400000000000000000000000000 */
				/* cos(x) = 0.dbb25c25b8260c14f6e7bc98ec991b70c65335198b0ab628bad20cc7b229d4dd62183cfa055 */
				new Quadruple(0x3ffe_b764_b84b_704c, 0x1829_edcf_7931_d932),
				new Quadruple(0x3f8b_b70c_6533_5198, 0xb0ab_628b_ad20_cc7b),
				/* sin(x) = 0.8369b434a372da7eb5c8a71fe36ce1e0b2b493f6f5cb2e38bcaec2a556b3678c401940d1c3c */
				new Quadruple(0x3ffe_06d3_6869_46e5, 0xb4fd_6b91_4e3f_c6da),
				new Quadruple(0xbf8b_e1f4_d4b6_c090, 0xa34d_1c74_3513_d5ab),

				/* x = 5.46875000000000000000000000000000000e-01 3ffe1800000000000000000000000000 */
				/* cos(x) = 0.daa9d20860827063fde51c09e855e9932e1b17143e7244fd267a899d41ae1f3bc6a0ec42e27 */
				new Quadruple(0x3ffe_b553_a410_c104, 0xe0c7_fbca_3813_d0ac),
				new Quadruple(0xbf8b_66cd_1e4e_8ebc, 0x18db_b02d_9857_662c),
				/* sin(x) = 0.852010f4f0800521378bd8dd614753d080c2e9e0775ffc609947b9132f5357404f464f06a58 */
				new Quadruple(0x3ffe_0a40_21e9_e100, 0x0a42_6f17_b1ba_c28f),
				new Quadruple(0xbf8c_617b_f9e8_b0fc, 0x4500_1cfb_35c2_3767),

				/* x = 5.54687500000000000000000000000000000e-01 3ffe1c00000000000000000000000000 */
				/* cos(x) = 0.d99ddd44e44a43d4d4a3a3ed95204106fd54d78e8c7684545c0da0b7c2c72be7a89b7c182ad */
				new Quadruple(0x3ffe_b33b_ba89_c894, 0x87a9_a947_47db_2a41),
				new Quadruple(0xbf8c_f7c8_1559_438b, 0x9c4b_dd5d_1f92_fa42),
				/* sin(x) = 0.86d45935ab396cb4e421e822dee54f3562dfcefeaa782184c23401d231f5ad981a1cc195b18 */
				new Quadruple(0x3ffe_0da8_b26b_5672, 0xd969_c843_d045_bdcb),
				new Quadruple(0xbf8c_8654_e901_880a, 0xac3e_f3d9_ee5f_f16e),

				/* x = 5.62500000000000000000000000000000000e-01 3ffe2000000000000000000000000000 */
				/* cos(x) = 0.d88e820b1526311dd561efbc0c1a9a5375eb26f65d246c5744b13ca26a7e0fd42556da843c8 */
				new Quadruple(0x3ffe_b11d_0416_2a4c, 0x623b_aac3_df78_1835),
				new Quadruple(0x3f8b_a537_5eb2_6f65, 0xd246_c574_4b13_ca27),
				/* sin(x) = 0.88868625b4e1dbb2313310133022527200c143a5cb16637cb7daf8ade82459ff2e98511f40f */
				new Quadruple(0x3ffe_110d_0c4b_69c3, 0xb764_6266_2026_6045),
				new Quadruple(0xbf8c_6c6f_f9f5_e2d1, 0xa74c_e41a_4128_3a91),

				/* x = 5.70312500000000000000000000000000000e-01 3ffe2400000000000000000000000000 */
				/* cos(x) = 0.d77bc4985e93a607c9d868b906bbc6bbe3a04258814acb0358468b826fc91bd4d814827f65e */
				new Quadruple(0x3ffe_aef7_8930_bd27, 0x4c0f_93b0_d172_0d78),
				new Quadruple(0xbf8c_ca20_e2fd_ed3b, 0xf5a9_a7e5_3dcb_a3ed),
				/* sin(x) = 0.8a3690fc5bfc11bf9535e2739a8512f448a41251514bbed7fc18d530f9b4650fcbb2861b0aa */
				new Quadruple(0x3ffe_146d_21f8_b7f8, 0x237f_2a6b_c4e7_350a),
				new Quadruple(0x3f8b_2f44_8a41_2515, 0x14bb_ed7f_c18d_5310),

				/* x = 5.78125000000000000000000000000000000e-01 3ffe2800000000000000000000000000 */
				/* cos(x) = 0.d665a937b4ef2b1f6d51bad6d988a4419c1d7051faf31a9efa151d7631117efac03713f950a */
				new Quadruple(0x3ffe_accb_526f_69de, 0x563e_daa3_75ad_b311),
				new Quadruple(0x3f8c_220c_e0eb_828f, 0xd798_d4f7_d0a8_ebb2),
				/* sin(x) = 0.8be472f9776d809af2b88171243d63d66dfceeeb739cc894e023fbc165a0e3f26ff729c5d57 */
				new Quadruple(0x3ffe_17c8_e5f2_eedb, 0x0135_e571_02e2_487b),
				new Quadruple(0xbf8b_c299_2031_1148, 0xc633_76b1_fdc0_43ea),

				/* x = 5.85937500000000000000000000000000000e-01 3ffe2c00000000000000000000000000 */
				/* cos(x) = 0.d54c3441844897fc8f853f0655f1ba695eba9fbfd7439dbb1171d862d9d9146ca5136f825ac */
				new Quadruple(0x3ffe_aa98_6883_0891, 0x2ff9_1f0a_7e0c_abe3),
				new Quadruple(0x3f8c_d34a_f5d4_fdfe, 0xba1c_edd8_8b8e_c317),
				/* sin(x) = 0.8d902565817ee7839bce3cd128060119492cd36d42d82ada30d7f8bde91324808377ddbf5d4 */
				new Quadruple(0x3ffe_1b20_4acb_02fd, 0xcf07_379c_79a2_500c),
				new Quadruple(0x3f87_1949_2cd3_6d42, 0xd82a_da30_d7f8_bde9),

				/* x = 5.93750000000000000000000000000000000e-01 3ffe3000000000000000000000000000 */
				/* cos(x) = 0.d42f6a1b9f0168cdf031c2f63c8d9304d86f8d34cb1d5fccb68ca0f2241427fc18d1fd5bbdf */
				new Quadruple(0x3ffe_a85e_d437_3e02, 0xd19b_e063_85ec_791b),
				new Quadruple(0x3f8b_304d_86f8_d34c, 0xb1d5_fccb_68ca_0f22),
				/* sin(x) = 0.8f39a191b2ba6122a3fa4f41d5a3ffd421417d46f19a22230a14f7fcc8fce5c75b4b28b29d1 */
				new Quadruple(0x3ffe_1e73_4323_6574, 0xc245_47f4_9e83_ab48),
				new Quadruple(0xbf84_5ef5_f415_c873, 0x2eee_e7af_5840_19b8),

				/* x = 6.01562500000000000000000000000000000e-01 3ffe3400000000000000000000000000 */
				/* cos(x) = 0.d30f4f392c357ab0661c5fa8a7d9b26627846fef214b1d19a22379ff9eddba087cf410eb097 */
				new Quadruple(0x3ffe_a61e_9e72_586a, 0xf560_cc38_bf51_4fb3),
				new Quadruple(0x3f8c_9331_3c23_7f79, 0x0a58_e8cd_111b_cffd),
				/* sin(x) = 0.90e0e0d81ca678796cc92c8ea8c2815bc72ca78abe571bfa8576aacc571e096a33237e0e830 */
				new Quadruple(0x3ffe_21c1_c1b0_394c, 0xf0f2_d992_591d_5185),
				new Quadruple(0x3f87_5bc7_2ca7_8abe, 0x571b_fa85_76aa_cc57),

				/* x = 6.09375000000000000000000000000000000e-01 3ffe3800000000000000000000000000 */
				/* cos(x) = 0.d1ebe81a95ee752e48a26bcd32d6e922d7eb44b8ad2232f6930795e84b56317269b9dd1dfa6 */
				new Quadruple(0x3ffe_a3d7_d035_2bdc, 0xea5c_9144_d79a_65ae),
				new Quadruple(0xbf8b_6dd2_814b_b475, 0x2ddc_d096_cf86_a17b),
				/* sin(x) = 0.9285dc9bc45dd9ea3d02457bcce59c4175aab6ff7929a8d287195525fdace200dba032874fb */
				new Quadruple(0x3ffe_250b_b937_88bb, 0xb3d4_7a04_8af7_99cb),
				new Quadruple(0x3f8b_c417_5aab_6ff7, 0x929a_8d28_7195_5260),

				/* x = 6.17187500000000000000000000000000000e-01 3ffe3c00000000000000000000000000 */
				/* cos(x) = 0.d0c5394d772228195e25736c03574707de0af1ca344b13bd3914bfe27518e9e426f5deff1e1 */
				new Quadruple(0x3ffe_a18a_729a_ee44, 0x5032_bc4a_e6d8_06af),
				new Quadruple(0xbf8c_c7c1_0fa8_71ae, 0x5da7_6216_375a_00ec),
				/* sin(x) = 0.94288e48bd0335fc41c4cbd2920497a8f5d1d8185c99fa0081f90c27e2a53ffdd208a0dbe69 */
				new Quadruple(0x3ffe_2851_1c91_7a06, 0x6bf8_8389_97a5_2409),
				new Quadruple(0x3f8b_7a8f_5d1d_8185, 0xc99f_a008_1f90_c27e),

				/* x = 6.25000000000000000000000000000000000e-01 3ffe4000000000000000000000000000 */
				/* cos(x) = 0.cf9b476c897c25c5bfe750dd3f308eaf7bcc1ed00179a256870f4200445043dcdb1974b5878 */
				new Quadruple(0x3ffe_9f36_8ed9_12f8, 0x4b8b_7fce_a1ba_7e61),
				new Quadruple(0x3f8a_d5ef_7983_da00, 0x2f34_4ad0_e1e8_4009),
				/* sin(x) = 0.95c8ef544210ec0b91c49bd2aa09e8515fa61a156ebb10f5f8c232a6445b61ebf3c2ec268f9 */
				new Quadruple(0x3ffe_2b91_dea8_8421, 0xd817_2389_37a5_5414),
				new Quadruple(0xbf8b_7aea_059e_5ea9, 0x144e_f0a0_73dc_d59c),

				/* x = 6.32812500000000000000000000000000000e-01 3ffe4400000000000000000000000000 */
				/* cos(x) = 0.ce6e171f92f2e27f32225327ec440ddaefae248413efc0e58ceee1ae369aabe73f88c87ed1a */
				new Quadruple(0x3ffe_9cdc_2e3f_25e5, 0xc4fe_6444_a64f_d888),
				new Quadruple(0x3f8a_bb5d_f5c4_9082, 0x7df8_1cb1_9ddc_35c7),
				/* sin(x) = 0.9766f93cd18413a6aafc1cfc6fc28abb6817bf94ce349901ae3f48c3215d3eb60acc5f78903 */
				new Quadruple(0x3ffe_2ecd_f279_a308, 0x274d_55f8_39f8_df85),
				new Quadruple(0x3f8a_576d_02f7_f299, 0xc693_2035_c7e9_1864),

				/* x = 6.40625000000000000000000000000000000e-01 3ffe4800000000000000000000000000 */
				/* cos(x) = 0.cd3dad1b5328a2e459f993f4f5108819faccbc4eeba9604e81c7adad51cc8a2561631a06826 */
				new Quadruple(0x3ffe_9a7b_5a36_a651, 0x45c8_b3f3_27e9_ea21),
				new Quadruple(0x3f8a_033f_5997_89dd, 0x752c_09d0_38f5_b5aa),
				/* sin(x) = 0.9902a58a45e27bed68412b426b675ed503f54d14c8172e0d373f42cadf04daf67319a7f94be */
				new Quadruple(0x3ffe_3205_4b14_8bc4, 0xf7da_d082_5684_d6cf),
				new Quadruple(0xbf8c_0957_e055_9759, 0xbf46_8f96_4605_e9a9),

				/* x = 6.48437500000000000000000000000000000e-01 3ffe4c00000000000000000000000000 */
				/* cos(x) = 0.cc0a0e21709883a3ff00911e11a07ee3bd7ea2b04e081be99be0264791170761ae64b8b744a */
				new Quadruple(0x3ffe_9814_1c42_e131, 0x0747_fe01_223c_2341),
				new Quadruple(0xbf87_1c42_815d_4fb1, 0xf7e4_1664_1fd9_b86f),
				/* sin(x) = 0.9a9bedcdf01b38d993f3d7820781de292033ead73b89e28f39313dbe3a6e463f845b5fa8490 */
				new Quadruple(0x3ffe_3537_db9b_e036, 0x71b3_27e7_af04_0f04),
				new Quadruple(0xbf8c_0eb6_fe60_a946, 0x23b0_eb86_3676_120e),

				/* x = 6.56250000000000000000000000000000000e-01 3ffe5000000000000000000000000000 */
				/* cos(x) = 0.cad33f00658fe5e8204bbc0f3a66a0e6a773f87987a780b243d7be83b3db1448ca0e0e62787 */
				new Quadruple(0x3ffe_95a6_7e00_cb1f, 0xcbd0_4097_781e_74cd),
				new Quadruple(0x3f8c_0735_3b9f_c3cc, 0x3d3c_0592_1ebd_f41e),
				/* sin(x) = 0.9c32cba2b14156ef05256c4f857991ca6a547cd7ceb1ac8a8e62a282bd7b9183648a462bd04 */
				new Quadruple(0x3ffe_3865_9745_6282, 0xadde_0a4a_d89f_0af3),
				new Quadruple(0x3f8b_1ca6_a547_cd7c, 0xeb1a_c8a8_e62a_282c),

				/* x = 6.64062500000000000000000000000000000e-01 3ffe5400000000000000000000000000 */
				/* cos(x) = 0.c99944936cf48c8911ff93fe64b3ddb7981e414bdaf6aae1203577de44878c62bc3bc9cf7b9 */
				new Quadruple(0x3ffe_9332_8926_d9e9, 0x1912_23ff_27fc_c968),
				new Quadruple(0xbf8c_1243_3f0d_f5a1, 0x284a_a8f6_fe54_410e),
				/* sin(x) = 0.9dc738ad14204e689ac582d0f85826590feece34886cfefe2e08cf2bb8488d55424dc9d3525 */
				new Quadruple(0x3ffe_3b8e_715a_2840, 0x9cd1_358b_05a1_f0b0),
				new Quadruple(0x3f8c_32c8_7f76_71a4, 0x4367_f7f1_7046_795e),

				/* x = 6.71875000000000000000000000000000000e-01 3ffe5800000000000000000000000000 */
				/* cos(x) = 0.c85c23c26ed7b6f014ef546c47929682122876bfbf157de0aff3c4247d820c746e32cd4174f */
				new Quadruple(0x3ffe_90b8_4784_ddaf, 0x6de0_29de_a8d8_8f25),
				new Quadruple(0x3f8b_6821_2287_6bfb, 0xf157_de0a_ff3c_4248),
				/* sin(x) = 0.9f592e9b66a9cf906a3c7aa3c10199849040c45ec3f0a747597311038101780c5f266059dbf */
				new Quadruple(0x3ffe_3eb2_5d36_cd53, 0x9f20_d478_f547_8203),
				new Quadruple(0x3f8b_9849_040c_45ec, 0x3f0a_7475_9731_1038),

				/* x = 6.79687500000000000000000000000000000e-01 3ffe5c00000000000000000000000000 */
				/* cos(x) = 0.c71be181ecd6875ce2da5615a03cca207d9adcb9dfb0a1d6c40a4f0056437f1a59ccddd06ee */
				new Quadruple(0x3ffe_8e37_c303_d9ad, 0x0eb9_c5b4_ac2b_407a),
				new Quadruple(0xbf8c_aefc_1329_1a31, 0x027a_f149_dfad_87fd),
				/* sin(x) = 0.a0e8a725d33c828c11fa50fd9e9a15ffecfad43f3e534358076b9b0f6865694842b1e8c67dc */
				new Quadruple(0x3ffe_41d1_4e4b_a679, 0x0518_23f4_a1fb_3d34),
				new Quadruple(0x3f8b_5ffe_cfad_43f3, 0xe534_3580_76b9_b0f7),

				/* x = 6.87500000000000000000000000000000000e-01 3ffe6000000000000000000000000000 */
				/* cos(x) = 0.c5d882d2ee48030c7c07d28e981e34804f82ed4cf93655d2365389b716de6ad44676a1cc5da */
				new Quadruple(0x3ffe_8bb1_05a5_dc90, 0x0618_f80f_a51d_303c),
				new Quadruple(0x3f8c_a402_7c17_6a67, 0xc9b2_ae91_b29c_4db9),
				/* sin(x) = 0.a2759c0e79c35582527c32b55f5405c182c66160cb1d9eb7bb0b7cdf4ad66f317bda4332914 */
				new Quadruple(0x3ffe_44eb_381c_f386, 0xab04_a4f8_656a_bea8),
				new Quadruple(0x3f89_7060_b198_5832, 0xc767_adee_c2df_37d3),

				/* x = 6.95312500000000000000000000000000000e-01 3ffe6400000000000000000000000000 */
				/* cos(x) = 0.c4920cc2ec38fb891b38827db08884fc66371ac4c2052ca8885b981bbcfd3bb7b093ee31515 */
				new Quadruple(0x3ffe_8924_1985_d871, 0xf712_3671_04fb_6111),
				new Quadruple(0x3f89_3f19_8dc6_b130, 0x814b_2a22_16e6_06ef),
				/* sin(x) = 0.a400072188acf49cd6b173825e038346f105e1301afe642bcc364cea455e21e506e3e927ed8 */
				new Quadruple(0x3ffe_4800_0e43_1159, 0xe939_ad62_e704_bc07),
				new Quadruple(0x3f88_a378_82f0_980d, 0x7f32_15e6_1b26_7523),

				/* x = 7.03125000000000000000000000000000000e-01 3ffe6800000000000000000000000000 */
				/* cos(x) = 0.c348846bbd3631338ffe2bfe9dd1381a35b4e9c0c51b4c13fe376bad1bf5caacc4542be0aa9 */
				new Quadruple(0x3ffe_8691_08d7_7a6c, 0x6267_1ffc_57fd_3ba2),
				new Quadruple(0x3f8c_c0d1_ada7_4e06, 0x28da_609f_f1bb_5d69),
				/* sin(x) = 0.a587e23555bb08086d02b9c662cdd29316c3e9bd08d93793634a21b1810cce73bdb97a99b9e */
				new Quadruple(0x3ffe_4b0f_c46a_ab76, 0x1010_da05_738c_c59c),
				new Quadruple(0xbf8c_6b67_49e0_b217, 0xb936_4364_e5ae_f274),

				/* x = 7.10937500000000000000000000000000000e-01 3ffe6c00000000000000000000000000 */
				/* cos(x) = 0.c1fbeef380e4ffdd5a613ec8722f643ffe814ec2343e53adb549627224fdc9f2a7b77d3d69f */
				new Quadruple(0x3ffe_83f7_dde7_01c9, 0xffba_b4c2_7d90_e45f),
				new Quadruple(0xbf8b_bc00_17eb_13dc, 0xbc1a_c524_ab69_d8de),
				/* sin(x) = 0.a70d272a76a8d4b6da0ec90712bb748b96dabf88c3079246f3db7eea6e58ead4ed0e2843303 */
				new Quadruple(0x3ffe_4e1a_4e54_ed51, 0xa96d_b41d_920e_2577),
				new Quadruple(0xbf8a_6e8d_24a8_0ee7, 0x9f0d_b721_8490_22b2),

				/* x = 7.18750000000000000000000000000000000e-01 3ffe7000000000000000000000000000 */
				/* cos(x) = 0.c0ac518c8b6ae710ba37a3eeb90cb15aebcb8bed4356fb507a48a6e97de9aa6d9660116b436 */
				new Quadruple(0x3ffe_8158_a319_16d5, 0xce21_746f_47dd_7219),
				new Quadruple(0x3f8c_8ad7_5e5c_5f6a, 0x1ab7_da83_d245_374c),
				/* sin(x) = 0.a88fcfebd9a8dd47e2f3c76ef9e2439920f7e7fbe735f8bcc985491ec6f12a2d4214f8cfa99 */
				new Quadruple(0x3ffe_511f_9fd7_b351, 0xba8f_c5e7_8edd_f3c5),
				new Quadruple(0xbf8c_e336_f840_c020, 0xc650_3a19_b3d5_b70a),

				/* x = 7.26562500000000000000000000000000000e-01 3ffe7400000000000000000000000000 */
				/* cos(x) = 0.bf59b17550a4406875969296567cf3e3b4e483061877c02811c6cae85fad5a6c3da58f49292 */
				new Quadruple(0x3ffe_7eb3_62ea_a148, 0x80d0_eb2d_252c_acfa),
				new Quadruple(0xbf8a_8389_636f_9f3c, 0xf107_fafd_c726_a2f4),
				/* sin(x) = 0.aa0fd66eddb921232c28520d3911b8a03193b47f187f1471ac216fbcd5bb81029294d3a73f1 */
				new Quadruple(0x3ffe_541f_acdd_bb72, 0x4246_5850_a41a_7223),
				new Quadruple(0x3f8c_c501_8c9d_a3f8, 0xc3f8_a38d_610b_7de7),

				/* x = 7.34375000000000000000000000000000000e-01 3ffe7800000000000000000000000000 */
				/* cos(x) = 0.be0413f84f2a771c614946a88cbf4da1d75a5560243de8f2283fefa0ea4a48468a52d51d8b3 */
				new Quadruple(0x3ffe_7c08_27f0_9e54, 0xee38_c292_8d51_197f),
				new Quadruple(0xbf8c_92f1_452d_54fe, 0xde10_b86e_be00_82f9),
				/* sin(x) = 0.ab8d34b36acd987210ed343ec65d7e3adc2e7109fce43d55c8d57dfdf55b9e01d2cc1f1b9ec */
				new Quadruple(0x3ffe_571a_6966_d59b, 0x30e4_21da_687d_8cbb),
				new Quadruple(0xbf87_c523_d18e_f603, 0x1bc2_aa37_2a82_020b),

				/* x = 7.42187500000000000000000000000000000e-01 3ffe7c00000000000000000000000000 */
				/* cos(x) = 0.bcab7e6bfb2a14a9b122c574a376bec98ab14808c64a4e731b34047e217611013ac99c0f25d */
				new Quadruple(0x3ffe_7956_fcd7_f654, 0x2953_6245_8ae9_46ed),
				new Quadruple(0x3f8c_f64c_558a_4046, 0x3252_7398_d9a0_23f1),
				/* sin(x) = 0.ad07e4c409d08c4fa3a9057bb0ac24b8636e74e76f51e09bd6b2319707cbd9f5e254643897a */
				new Quadruple(0x3ffe_5a0f_c988_13a1, 0x189f_4752_0af7_6158),
				new Quadruple(0x3f8c_25c3_1b73_a73b, 0x7a8f_04de_b591_8cb8),

				/* x = 7.50000000000000000000000000000000000e-01 3ffe8000000000000000000000000000 */
				/* cos(x) = 0.bb4ff632a908f73ec151839cb9d993b4e0bfb8f20e7e44e6e4aee845e35575c3106dbe6fd06 */
				new Quadruple(0x3ffe_769f_ec65_5211, 0xee7d_82a3_0739_73b3),
				new Quadruple(0x3f8b_3b4e_0bfb_8f20, 0xe7e4_4e6e_4aee_845e),
				/* sin(x) = 0.ae7fe0b5fc786b2d966e1d6af140a488476747c2646425fc7533f532cd044cb10a971a49a6a */
				new Quadruple(0x3ffe_5cff_c16b_f8f0, 0xd65b_2cdc_3ad5_e281),
				new Quadruple(0x3f8c_2442_3b3a_3e13, 0x2321_2fe3_a99f_a996),

				/* x = 7.57812500000000000000000000000000000e-01 3ffe8400000000000000000000000000 */
				/* cos(x) = 0.b9f180ba77dd0751628e135a9508299012230f14becacdd14c3f8862d122de5b56d55b53360 */
				new Quadruple(0x3ffe_73e3_0174_efba, 0x0ea2_c51c_26b5_2a10),
				new Quadruple(0x3f8c_4c80_9118_78a5, 0xf656_6e8a_61fc_4317),
				/* sin(x) = 0.aff522a954f2ba16d9defdc416e33f5e9a5dfd5a6c228e0abc4d521327ff6e2517a7b3851dd */
				new Quadruple(0x3ffe_5fea_4552_a9e5, 0x742d_b3bd_fb88_2dc6),
				new Quadruple(0x3f8c_faf4_d2ef_ead3, 0x6114_7055_e26a_9099),

				/* x = 7.65625000000000000000000000000000000e-01 3ffe8800000000000000000000000000 */
				/* cos(x) = 0.b890237d3bb3c284b614a0539016bfa1053730bbdf940fa895e185f8e58884d3dda15e63371 */
				new Quadruple(0x3ffe_7120_46fa_7767, 0x8509_6c29_40a7_202d),
				new Quadruple(0x3f8c_fd08_29b9_85de, 0xfca0_7d44_af0c_2fc7),
				/* sin(x) = 0.b167a4c90d63c4244cf5493b7cc23bd3c3c1225e078baa0c53d6d400b926281f537a1a260e6 */
				new Quadruple(0x3ffe_62cf_4992_1ac7, 0x8848_99ea_9276_f984),
				new Quadruple(0x3f8c_de9e_1e09_12f0, 0x3c5d_5062_9eb6_a006),

				/* x = 7.73437500000000000000000000000000000e-01 3ffe8c00000000000000000000000000 */
				/* cos(x) = 0.b72be40067aaf2c050dbdb7a14c3d7d4f203f6b3f0224a4afe55d6ec8e92b508fd5c5984b3b */
				new Quadruple(0x3ffe_6e57_c800_cf55, 0xe580_a1b7_b6f4_2988),
				new Quadruple(0xbf8c_4158_6fe0_4a60, 0x7eed_ada8_0d51_489c),
				/* sin(x) = 0.b2d7614b1f3aaa24df2d6e20a77e1ca3e6d838c03e29c1bcb026e6733324815fadc9eb89674 */
				new Quadruple(0x3ffe_65ae_c296_3e75, 0x5449_be5a_dc41_4efc),
				new Quadruple(0x3f8b_ca3e_6d83_8c03, 0xe29c_1bcb_026e_6733),

				/* x = 7.81250000000000000000000000000000000e-01 3ffe9000000000000000000000000000 */
				/* cos(x) = 0.b5c4c7d4f7dae915ac786ccf4b1a498d3e73b6e5e74fe7519d9c53ee6d6b90e881bddfc33e1 */
				new Quadruple(0x3ffe_6b89_8fa9_efb5, 0xd22b_58f0_d99e_9635),
				new Quadruple(0xbf8c_b396_0c62_48d0, 0xc580_c573_131d_608d),
				/* sin(x) = 0.b44452709a59752905913765434a59d111f0433eb2b133f7d103207e2aeb4aae111ddc385b3 */
				new Quadruple(0x3ffe_6888_a4e1_34b2, 0xea52_0b22_6eca_8695),
				new Quadruple(0xbf8c_3177_707d_e60a, 0x6a76_6041_77e6_fc0f),

				/* x = 7.89062500000000000000000000000000000e-01 3ffe9400000000000000000000000000 */
				/* cos(x) = 0.b45ad4975b1294cadca4cf40ec8f22a68cd14b175835239a37e63acb85e8e9505215df18140 */
				new Quadruple(0x3ffe_68b5_a92e_b625, 0x2995_b949_9e81_d91e),
				new Quadruple(0x3f8c_1534_668a_58ba, 0xc1a9_1cd1_bf31_d65c),
				/* sin(x) = 0.b5ae7285bc10cf515753847e8f8b7a30e0a580d929d770103509880680f7b8b0e8ad23b65d8 */
				new Quadruple(0x3ffe_6b5c_e50b_7821, 0x9ea2_aea7_08fd_1f17),
				new Quadruple(0xbf89_73c7_d69f_c9b5, 0x8a23_fbf2_bd9d_fe60),
			};
		}
	}
}
